using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IVisitorRepository
    {
        Task CreateAsync(Visitor visit, CancellationToken ct = default);
        Task<List<Visitor>> ReadAllVisitorsAsync(CancellationToken ct = default);
        Task<Visitor?> ReadByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Visitor>> ReadByUrlIdAsync(Guid urlId, CancellationToken ct = default);
        Task<List<Visitor>> ReadVisitsByUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct = default);
        Task<List<Visitor>> ReadByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<int> GetVisitCountByUrlCodeAsync(string code, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}