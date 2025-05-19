using Microsoft.EntityFrameworkCore;
using ShrinkLink.Auth.Service.Models.Entities;
using ShrinkLink.Auth.Service.Models.Interfaces;

namespace ShrinkLink.Auth.Service.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> FetchRolesIdAsync(string name,CancellationToken ct = default)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == name, ct);
            return role.Id;
        }
    }
}
