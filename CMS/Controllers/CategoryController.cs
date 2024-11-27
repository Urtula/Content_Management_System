using CMS.Application.DTO;
using CMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Get all categories
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // Get category by Name
        [HttpGet]
        public async Task<IActionResult> GetCategoryByName([FromQuery] string CategoryName)
        {
            var category = await _categoryService.GetCategoryByNameAsync(CategoryName);
            if (category == null)
            {
                return NotFound(); // Optionally handle case where category isn't found
            }
            return Ok(category);
        }

        // Add a new category
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                return BadRequest("Category data is required.");
            }
            var categoryExists = await _categoryService.DoesCategoryExistAsync(categoryDTO.CategoryName);
            if (categoryExists == true)
            {
                await _categoryService.UpdateCategoryAsync(categoryDTO);
                return Ok(categoryDTO);
            }
            await _categoryService.AddCategoryAsync(categoryDTO);
            return CreatedAtAction(nameof(GetCategoryByName), new { id = categoryDTO.CategoryName }, categoryDTO);
        }

        // Delete a category
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory([FromQuery] string categoryName)
        {
            var success = await _categoryService.DeleteCategoryAsync(categoryName);
            if (!success)
            {
                return NotFound(); // Optionally handle case where category isn't found
            }
            return NoContent();
        }
    }
}
