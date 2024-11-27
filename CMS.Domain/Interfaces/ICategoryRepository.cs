using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> GetIdByNameAsync(string name);
        Task<bool> DoesCategoryExistAsync(string categoryName);
    }
}
