using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IVisitorService
    {
        Task<VisitsResponseTransferObject> FetchAllVisitsByLinkCodeAsync(Guid userId, string code, CancellationToken ct = default);
        Task<VisitsResponseTransferObject> FetchAllVisitsByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
