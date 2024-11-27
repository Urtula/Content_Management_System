using CMS.Application.DTO;
using CMS.Application.Interfaces;
using CMS.Application.Services;
using CMS.Domain.Entities;
using CMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("ByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"User with email {email} not found.");
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            try
            {
                // If categories are not provided, simply call CreateUserAsync without categories
                await _userService.CreateUserAsync(userDto, userDto.CategoryIDs);

                return Ok("User created successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Delete a category
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(UserDTO userDTO)
        {
            await _userService.DeleteUserAsync(userDTO.Email);
            return NoContent();
        }
        [HttpPost("{userId}/categories")]
        public async Task<IActionResult> AddCategoriesToUser(int userId, [FromBody] List<int> categoryIds)
        {
            try
            {
                await _userService.AddCategoriesToUserAsync(userId, categoryIds);
                return Ok($"Categories successfully added to user with ID {userId}.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{userId}/Categories")]
        public async Task<IActionResult> GetCategoriesByUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var categories = await _userService.GetCategoriesByUserAsync(user);

            if (categories == null || !categories.Any())
            {
                return NotFound($"No categories found for user with ID {userId}");
            }

            return Ok(categories);
        }
    }

}
