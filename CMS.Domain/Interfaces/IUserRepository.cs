using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<Category>> GetCategoriesByUserAsync(int userId);
        Task AddCategoriesToUserAsync(int userId, List<int> categoryIds);
    }
}
