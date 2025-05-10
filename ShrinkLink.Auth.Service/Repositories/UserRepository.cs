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

        public async Task CreateAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> IsUserExistsByUsername(string username, CancellationToken ct)
        {
            return await _context.Users.AnyAsync(x => x.Username == username, ct);
        }
    }
}
