using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default);
    }
}
