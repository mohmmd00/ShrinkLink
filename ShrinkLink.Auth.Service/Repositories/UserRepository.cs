using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ShrinkLink.Auth.Service.Models.Entities;
using ShrinkLink.Auth.Service.Models.Interfaces;

namespace ShrinkLink.Auth.Service.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> IsUserExistsByUsername(string username, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Username == username, ct);
        }


        public async Task<User> FetchUserByUsername(string username, CancellationToken ct = default)
        {
            var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == username, ct);
            return selectedUser;
        }

        public async Task<User> FetchUserById(string id, CancellationToken ct = default)
        {
            var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == id, ct);
            return selectedUser;
        }


        public async Task<bool> IsUserExistsByEmail(string email, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Email == email, ct);
        }
    }
}
