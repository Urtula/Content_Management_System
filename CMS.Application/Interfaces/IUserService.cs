using CMS.Application.DTO;
using CMS.Domain.Entities;
using CMS.Domain.Interfaces;
using CMS.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<Category>> GetCategoriesByUserAsync(UserDTO userDTO);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task CreateUserAsync(UserDTO userDto, List<int> categoryIds = null);
        Task AddCategoriesToUserAsync(int userId, List<int> categoryIds);
        Task DeleteUserAsync(string Email);
    }
}
