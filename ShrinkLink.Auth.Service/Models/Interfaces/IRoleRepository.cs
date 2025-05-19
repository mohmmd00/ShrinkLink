using ShrinkLink.Auth.Service.Models.Entities;

namespace ShrinkLink.Auth.Service.Models.Interfaces
{
    public interface IRoleRepository
    {
         Task CreateAsync(Role role);
         Task<Guid> FetchRolesIdAsync(string name, CancellationToken ct = default);
    }
}
