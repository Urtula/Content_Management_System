using CMS.Domain.Entities;
using CMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Interfaces
{
    public interface IContentRepository : IRepository<Content>
    {
        Task<Content> GetContentByIdAsync(int id);
        Task<IEnumerable<Content>> GetContentsByCategoryAsync(string categoryName);
        Task AddContentAsync(Content content);
        
    }
}
