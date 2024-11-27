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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CacheManager _cacheManager;

        public CategoryService(ICategoryRepository categoryRepository, CacheManager cacheManager)
        {
            _categoryRepository = categoryRepository;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            string cacheKey = "AllCategories";

            var cachedCategories = await _cacheManager.GetOrSetAsync(cacheKey, async () =>
            {
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Adapt<IEnumerable<CategoryDTO>>();
            }, TimeSpan.FromMinutes(10));

            return cachedCategories;
        }

        public async Task<CategoryDTO> GetCategoryByNameAsync(string categoryName)
        {
            string cacheKey = $"Category:{categoryName}";

            var cachedCategory = await _cacheManager.GetOrSetAsync(cacheKey, async () =>
            {
                var categoryid = await _categoryRepository.GetIdByNameAsync(categoryName);
                var category = await _categoryRepository.GetByIdAsync(categoryid);
                if (category == null)
                {
                    throw new KeyNotFoundException("Category not found");
                }

                return category.Adapt<CategoryDTO>();
            }, TimeSpan.FromMinutes(10));

            return cachedCategory;
        }

        public async Task AddCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = categoryDTO.Adapt<Category>();
            await _categoryRepository.AddAsync(category);

            await _cacheManager.RemoveAsync("AllCategories");  
        }
        public async Task<bool> DoesCategoryExistAsync(string categoryName)
        {
            return await _categoryRepository.DoesCategoryExistAsync(categoryName);
        }
        public async Task UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var categoryid = await _categoryRepository.GetIdByNameAsync(categoryDTO.CategoryName);
            var category = await _categoryRepository.GetByIdAsync(categoryid);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            categoryDTO.Adapt(category);
            await _categoryRepository.UpdateAsync(category, category.Id);

            await _cacheManager.RemoveAsync($"Category:{categoryDTO.CategoryName}");
            await _cacheManager.RemoveAsync("AllCategories");
        }

        public async Task<bool> DeleteCategoryAsync(string categoryName)
        {
            var categoryid = await _categoryRepository.GetIdByNameAsync(categoryName);
            var category = await _categoryRepository.GetByIdAsync(categoryid);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(category, category.Id);

            await _cacheManager.RemoveAsync($"Category:{categoryName}");
            await _cacheManager.RemoveAsync("AllCategories");
            return true;
        }
    }
}
