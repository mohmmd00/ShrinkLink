namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IAuthServiceHttpRequestHandler
    {
        Task<HttpResponseMessage> GetUserBaseInformationAsync(string token, CancellationToken ct = default);

    }
}
