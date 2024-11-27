using CMS.Domain.Entities;
using CMS.Domain.Interfaces;
using CMS.Infrastructure.Data;
using CMS.Utilies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CMSDbContext _dbContext;
        private readonly CacheManager _cacheManager;

        public CategoryRepository(CMSDbContext dbContext, CacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }
        public async Task<int> GetIdByNameAsync(string name)
        {
            var category = await _dbContext.Categories
                                .Where(c => c.CategoryName.ToLower() == name.ToLower())
                                .FirstOrDefaultAsync();
            if(category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            return category.Id;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            string cacheKey = "AllCategories";

            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () => await _dbContext.Categories.ToListAsync(),
                TimeSpan.FromMinutes(10)
            );
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            string cacheKey = $"Category:{id}";

            return await _cacheManager.GetOrSetAsync(cacheKey,
                async () => await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == id),
                TimeSpan.FromMinutes(10) 
            );
        }
        public async Task<bool> DoesCategoryExistAsync(string categoryName)
        {
            return await _dbContext.Categories.AnyAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }
        public async Task AddAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            await _cacheManager.RemoveAsync("AllCategories");
        }

        public async Task UpdateAsync(Category category, int id)
        {
            var existingCategory = await _dbContext.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                throw new ArgumentException("Category not found.");
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDesc = category.CategoryDesc;

            await _dbContext.SaveChangesAsync();

            await _cacheManager.RemoveAsync($"Category:{id}");
            await _cacheManager.RemoveAsync("AllCategories");
        }

        public async Task DeleteAsync(Category _category, int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            await _cacheManager.RemoveAsync($"Category:{id}");
            await _cacheManager.RemoveAsync("AllCategories");
        }
    }
}
