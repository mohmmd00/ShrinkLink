namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IFindIpServiceHttpRequestHandler
    {
        Task<HttpResponseMessage> PostIpAddressToIpApi(string ipAddress, CancellationToken ct = default);
    }
}
