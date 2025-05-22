using AuthService.UrlShortener.Contracts.Dtos;
using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default);
        Task<UserBasicInformation> GetUserBaseInformationAsync(string asndca, CancellationToken ct = default);
    }
}
