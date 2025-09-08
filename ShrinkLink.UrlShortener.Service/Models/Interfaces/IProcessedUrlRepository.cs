using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IProcessedUrlRepository
    {
        Task<bool> CreateAsync(ProcessedUrl link, CancellationToken ct = default);
        Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default);
        Task<ProcessedUrl> FetchWantedUrl(string code, CancellationToken ct = default);
        Task<List<ProcessedUrl>> FetchAllProcessedUrlsByUserId(Guid userId, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid userId, string code, CancellationToken ct = default);
        Task<bool> DeleteAllByUserIdAsync(Guid userId, CancellationToken ct = default);

    }
}
