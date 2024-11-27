using CMS.Domain.Interfaces;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMS.Infrastructure.Data;
using CMS.Utilies;

namespace CMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CMSDbContext _dbContext;
        private readonly CacheManager _cacheManager;

        public UserRepository(CMSDbContext dbContext, CacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user, int id)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;

                _dbContext.Users.Update(existingUser);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(User user, int id)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            if (existingUser != null)
            {
                _dbContext.Users.Remove(existingUser);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string cacheKey = $"UserByEmail:{email}";

            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await _dbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == email);
                },
                TimeSpan.FromMinutes(5)
            );
        }
        public async Task AddCategoriesToUserAsync(int userId, List<int> categoryIds)
        {
            // Fetch the user
            var user = await _dbContext.Users
                .Include(u => u.Categories) 
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            
            var categories = await _dbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ArgumentException("No valid categories found for the provided IDs.");
            }
            
            foreach (var category in categories)
            {
                if (!user.Categories.Any(c => c.Id == category.Id))
                {
                    user.Categories.Add(category);
                }
            }
            await _dbContext.SaveChangesAsync();
            await _cacheManager.RemoveAsync($"CategoriesByUser:{userId}");
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUserAsync(int userId)
        {
            string cacheKey = $"CategoriesByUser:{userId}";

            return await _cacheManager.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await _dbContext.Users
                        .Where(u => u.Id == userId)
                        .SelectMany(u => u.Categories)
                        .Distinct()
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(5) // Cache for 5 minutes
            );
        }
    }

}
