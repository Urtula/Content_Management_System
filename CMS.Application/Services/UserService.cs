using CMS.Application.DTO;
using CMS.Application.Interfaces;
using CMS.Domain.Entities;
using CMS.Domain.Interfaces;
using CMS.Utilies;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CacheManager _cacheManager;
        public UserService(IUserRepository userRepository, ICategoryRepository categoryRepository, CacheManager cacheManager)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _cacheManager = cacheManager;
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            string cacheKey = $"User:{id}";

            return await _cacheManager.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var user = await _userRepository.GetByIdAsync(id);
                    if (user == null)
                    {
                        throw new KeyNotFoundException($"User with ID {id} not found.");
                    }
                    return user.Adapt<UserDTO>();
                },
                TimeSpan.FromMinutes(5)
            );
        }

        // Get all users using cache
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            string cacheKey = "AllUsers";

            return await _cacheManager.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var users = await _userRepository.GetAllAsync();
                    return users.Adapt<IEnumerable<UserDTO>>();
                },
                TimeSpan.FromMinutes(10)
            );
        }

        // Get User by mail using cache
        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            string cacheKey = $"UserByEmail:{email}";

            return await _cacheManager.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var user = await _userRepository.GetUserByEmailAsync(email);
                    if (user == null)
                    {
                        throw new KeyNotFoundException($"User with email {email} not found.");
                    }
                    return user.Adapt<UserDTO>();
                },
                TimeSpan.FromMinutes(5)
            );
        }

        // Get Categories using cache
        public async Task<IEnumerable<Category>> GetCategoriesByUserAsync(UserDTO userDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(userDTO.Email);

            if (user == null)
            {
                throw new KeyNotFoundException($"User not found.");
            }

            string cacheKey = $"CategoriesByUser:{user.Id}";

            return await _cacheManager.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var categories = await _userRepository.GetCategoriesByUserAsync(user.Id);
                    return categories;
                },
                TimeSpan.FromMinutes(5)
            );
        }

        // Create User and delete cahce
        public async Task CreateUserAsync(UserDTO userDto, List<int> categoryIds = null)
        {
            var user = userDto.Adapt<User>();

            // Check if categories are provided
            if (categoryIds != null && categoryIds.Any())
            {
                // Fetch categories from the database using the provided category IDs
                List<Category> categories = new List<Category>();
                foreach (var item in categoryIds)
                {
                    var category = await _categoryRepository.GetByIdAsync(item);
                    if(category == null)
                    {
                        categories.Add(category);
                    }
                }

                if (categories.Count() != categoryIds.Count)
                {
                    throw new KeyNotFoundException("One or more categories not found.");
                }

                user.Categories = categories.ToList();
            }

            await _userRepository.AddAsync(user);

            await _cacheManager.RemoveAsync("AllUsers");
        }
        public async Task AddCategoriesToUserAsync(int userId, List<int> categoryIds)
        {
            await _userRepository.AddCategoriesToUserAsync(userId, categoryIds);
        }
        // Delete User and cache
        public async Task DeleteUserAsync(string Email)
        {
            var user = await _userRepository.GetUserByEmailAsync(Email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            await _userRepository.DeleteAsync(user, user.Id);

            // Invalidate cache for all since user is deleted
            await _cacheManager.RemoveAsync($"User:{Email}");
            await _cacheManager.RemoveAsync("AllUsers");
        }
    }
}
