using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IShortenUrlRepository
    {
        Task CreateAsync(ShortenUrl link, CancellationToken ct = default);
        Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default);
        Task<ShortenUrl> FetchWantedUrl(string code, CancellationToken ct = default);
    }
}
