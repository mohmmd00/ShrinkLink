using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.Analyze.Service.Models.Interfaces
{
    public interface IShortenerServiceHttpRequestHandler
    {
        Task<HttpResponseMessage> GetAllVisitorsByProcessedLinkAsync(string code, string token, CancellationToken ct = default);
        Task<HttpResponseMessage> GetAllVisitorsByUserIdAsync(string token, CancellationToken ct = default);

    }
}
