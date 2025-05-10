using ShrinkLink.Auth.Service.Models.Entities;
namespace ShrinkLink.Auth.Service.Models.Interfaces
{
    public interface IUserRepository
    {
         Task CreateAsync(User user , CancellationToken ct);
         Task<bool> IsUserExistsByUsername(string username, CancellationToken ct);

    }
}
