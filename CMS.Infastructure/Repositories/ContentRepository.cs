using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Domain.Interfaces;
using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CMS.Infrastructure.Data;
using CMS.Utilies;
using CMS.Domain.Enums;

namespace CMS.Infrastructure.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly CMSDbContext _dbContext;
        private readonly CacheManager _cacheManager;
        private readonly ICategoryRepository _categoryRepository;
        public ContentRepository(CMSDbContext dbContext, CacheManager cacheManager, ICategoryRepository categoryRepository)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _categoryRepository = categoryRepository;
        }

        public async Task<Content?> GetByIdAsync(int id)
        {
            string cacheKey = $"Content:{id}";

            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await _dbContext.Contents
                        .Include(c => c.Variants)
                        .FirstOrDefaultAsync(c => c.Id == id);
                },
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<IEnumerable<Content>> GetAllAsync()
        {
            return await _dbContext.Contents
                .Include(c => c.Variants)
                .ToListAsync();
        }

        public async Task AddAsync(Content content)
        {
            //Cheat workaround to add new content with variants given.
            List<Variant> variants = content.Variants.ToList();
            content.Variants = null;
            await _dbContext.Contents.AddAsync(content);

            await _dbContext.SaveChangesAsync();

            foreach (var item in variants)
            {
                item.ContentId = content.Id;

                // Add the variants
                await _dbContext.Variants.AddAsync(item);
            }

            await _dbContext.SaveChangesAsync();
            await _cacheManager.RemoveAsync($"Content:{content.Id}");
        }

        public async Task UpdateAsync(Content content, int id)
        {
            var existingContent = await _dbContext.Contents.FindAsync(id);
            if (existingContent != null)
            {
                existingContent.Title = content.Title;
                existingContent.Description = content.Description;
                existingContent.CategoryId = content.CategoryId;

                _dbContext.Contents.Update(existingContent);
                await _dbContext.SaveChangesAsync();

                // Invalidate cache after update
                await _cacheManager.RemoveAsync($"Content:{id}");
            }
        }

        public async Task DeleteAsync(Content content, int id)
        {
            var existingContent = await _dbContext.Contents.FindAsync(id);
            if (existingContent != null)
            {
                _dbContext.Contents.Remove(existingContent);
                await _dbContext.SaveChangesAsync();

                // Invalidate cache after delete
                await _cacheManager.RemoveAsync($"Content:{id}");
            }
        }

        public async Task<IEnumerable<Content>> GetContentsByCategoryAsync(string categoryName)
        {
            string cacheKey = $"ContentByCategory:{categoryName}";
            var categoryid = await _categoryRepository.GetIdByNameAsync(categoryName);
            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await _dbContext.Contents
                        .Where(c => c.CategoryId == categoryid)
                        .Include(c => c.Variants)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(5)
            );
        }
        public async Task<Content> GetContentByIdAsync(int id)
        {
            string cacheKey = $"Content:{id}";

            // Check cache first, if not available fetch from the database
            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () => await _dbContext.Contents
                    .Include(c => c.Variants)
                    .FirstOrDefaultAsync(c => c.Id == id),
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<IEnumerable<Variant>> GetVariantsByContentAsync(int contentId)
        {
            string cacheKey = $"VariantsByContent:{contentId}";

            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await _dbContext.Variants
                        .Where(v => v.ContentId == contentId)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(5)
            );
        }
        public async Task AddContentAsync(Content content)
        {
            // Check if the category exists
            var category = await _dbContext.Categories.FindAsync(content.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category does not exist.");
            }

            // Add content to the database
            await _dbContext.Contents.AddAsync(content);
            await _dbContext.SaveChangesAsync();

            // Invalidate related cache if needed
            await _cacheManager.RemoveAsync($"ContentByCategory:{category.CategoryName}");
        }
    }

}
