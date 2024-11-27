using CMS.Application.Interfaces;
using CMS.Application.Services;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Application.DTO;
using CMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ICategoryService _categoryService;

        public ContentController(IContentService contentService, ICategoryService categoryService)
        {
            _contentService = contentService;
            _categoryService = categoryService;
        }

        [HttpGet("ByCategory/{categoryName}")]
        public async Task<IActionResult> GetContentByCategory(string categoryName)
        {
            var content = await _contentService.GetContentsByCategoryAsync(categoryName);

            if (content == null || !content.Any())
            {
                return NotFound($"No content found for {categoryName}");
            }

            return Ok(content);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(int id)
        {
            var content = await _contentService.GetContentByIdAsync(id);
            if (content == null)
            {
                return NotFound(); // Return 404 if content is not found
            }

            return Ok(content); // Return the content with 200 OK status
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _contentService.GetContentByIdAsync(id);
            if (content == null)
            {
                return NotFound(); // Return 404 if content is not found
            }
            await _contentService.DeleteContentAsync(id);
            return Ok(content); // Return the content with 200 OK status
        }
        // Add Content
        [HttpPost]
        public async Task<IActionResult> AddContent([FromBody] ContentDTO content)
        {
            if (content == null)
            {
                return BadRequest("Content cannot be null");
            }

            // Check if the category exists
            var category = await _categoryService.GetCategoryByNameAsync(content.CategoryName);
            if (category == null)
            {
                return BadRequest($"Category with ID {content.CategoryName} does not exist.");
            }

            // Add the content and save changes
            await _contentService.AddContentAsync(content);

            // Return a CreatedAtAction result to respond with the newly created content
            return CreatedAtAction(nameof(GetContentById), new { id = content.Id }, content);
        }
    }
}
