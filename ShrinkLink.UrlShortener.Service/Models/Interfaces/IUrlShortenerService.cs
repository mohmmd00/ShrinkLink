using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<LinkShortenerResponseTransferObject> GenerateShortLink(LinkShortenerRequestTransferObject request, Guid userPrimaryId, CancellationToken ct = default);

        Task<AllLinksFetcherResponseTransferObject> FetchAllProcessedLinks(Guid userPrimaryId, CancellationToken ct = default);

        Task<LinkRemoverResponseTransferObject> RemoveSelectedLink(Guid userPrimaryId, string shortenedLinkCode,
            CancellationToken ct = default);

        Task<LinkCheckerResponseTransferObject> CheckSelectedLink(Guid userPrimaryId, string code,
            CancellationToken ct = default);
    }
}
