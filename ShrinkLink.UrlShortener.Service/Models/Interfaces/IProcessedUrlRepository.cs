using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IProcessedUrlRepository
    {
        Task CreateAsync(ProcessedUrl link, CancellationToken ct = default);
        Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default);
        Task<ProcessedUrl> FetchWantedUrl(string code, CancellationToken ct = default);
    }
}
