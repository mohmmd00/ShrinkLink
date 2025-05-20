using ShrinkLink.Auth.Service.Models.Entities;
namespace ShrinkLink.Auth.Service.Models.Interfaces
{
    public interface IUserRepository
    {
         Task CreateAsync(User user , CancellationToken ct);
         Task<bool> IsUserExistsByUsername(string username, CancellationToken ct);
         Task<User> FetchUserByUsername(string username, CancellationToken ct = default);
         Task<User> FetchUserById(string id, CancellationToken ct = default);

    }
}
