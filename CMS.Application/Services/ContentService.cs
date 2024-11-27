using CMS.Application.DTO;
using CMS.Application.Interfaces;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
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
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CacheManager _cacheManager;

        public ContentService(IContentRepository contentRepository, ICategoryRepository categoryRepository, CacheManager cacheManager)
        {
            _contentRepository = contentRepository;
            _categoryRepository = categoryRepository;
            _cacheManager = cacheManager;
        }

        public async Task<ContentDTO?> GetContentByIdAsync(int id)
        {
            // Check if the content is cached
            string cacheKey = $"Content:{id}";
            var content = await _cacheManager.GetOrSetAsync(cacheKey,
                async () => await _contentRepository.GetContentByIdAsync(id),
                TimeSpan.FromMinutes(5)
            );

            // Map the Content entity to ContentDTO
            return content.Adapt<ContentDTO?>();
            
        }

        public async Task AddContentAsync(ContentDTO contentDto)
        {
            // Check if the category exists
            var categoryid = await _categoryRepository.GetIdByNameAsync(contentDto.CategoryName);
            var category = await _categoryRepository.GetByIdAsync(categoryid);
            if (category == null)
            {
                throw new Exception($"Category with ID {contentDto.CategoryName} does not exist.");
            }

            // Map ContentDTO to Content entity
            var content = contentDto.Adapt<Content?>();
            content.CategoryId = categoryid;
            // Add the content and save changes
            await _contentRepository.AddAsync(content);

            // Invalidate cache for content if necessary
            await _cacheManager.RemoveAsync($"Content:{content.Id}");
        }

        public async Task<IEnumerable<ContentDTO>> GetContentsByCategoryAsync(string categoryName)
        {
            var contents = await _contentRepository.GetContentsByCategoryAsync(categoryName);

            return contents.Adapt<IEnumerable<ContentDTO>>();
        }

        public async Task DeleteContentAsync(int contentId)
        {
            var content = await _contentRepository.GetByIdAsync(contentId);
            if (content == null)
                throw new KeyNotFoundException("Content not found");

            await _contentRepository.DeleteAsync(content, content.Id);

            // Invalidate cache as content has been deleted
            await _cacheManager.RemoveAsync($"Content:{contentId}");
            await _cacheManager.RemoveAsync("AllContents");
        }
    }
}
