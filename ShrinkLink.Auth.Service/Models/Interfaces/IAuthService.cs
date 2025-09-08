using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;

namespace ShrinkLink.Auth.Service.Models.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseTransferObject> Register(RegisterRequestTransferObject request , CancellationToken ct);
        Task<LoginResponseTransferObject> Login(LoginRequestTransferObject request , CancellationToken ct);
        Task<UserBasicInformationTransferObject> UserBasicDetails(string token, CancellationToken ct);
    }
}
