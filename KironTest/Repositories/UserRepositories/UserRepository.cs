using KironTest.Entities;
using KironTest.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace KironTest.Repositories.UserRepositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUser(User user)
        {
            if (await UsernameExists(user.Username))
            {
                throw new InvalidOperationException($"Username '{user.Username}' already exists.");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> UsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}