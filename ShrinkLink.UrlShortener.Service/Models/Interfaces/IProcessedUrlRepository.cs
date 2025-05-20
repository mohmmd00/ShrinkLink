using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IProcessedUrlRepository
    {
        Task CreateAsync(ProccessedUrl link, CancellationToken ct = default);
        Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default);
        Task<ProccessedUrl> FetchWantedUrl(string code, CancellationToken ct = default);
    }
}
