using AuthService.Webapp.Contracts.Dtos;
using ShrinkLink.Auth.Service.Models.Entities;

namespace ShrinkLink.Auth.Service.Models.Interfaces
{
    public interface IAuthorizationService
    {
        Task<RegisterResponseTransferObject> Register(RegisterTransferObject request , CancellationToken ct);
        Task<LoginResponseTransferObject> Login(LoginTransferObject request , CancellationToken ct);
    }
}
