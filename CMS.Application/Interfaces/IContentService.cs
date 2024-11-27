using CMS.Application.DTO;
using CMS.Domain.Enums;
using CMS.Domain.Interfaces;
using CMS.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.Interfaces
{
    public interface IContentService
    {
        Task<IEnumerable<ContentDTO>> GetContentsByCategoryAsync(string categoryName);
        Task<ContentDTO> GetContentByIdAsync(int id);
        Task DeleteContentAsync(int contentId);
        Task AddContentAsync(ContentDTO content);
    }
}
