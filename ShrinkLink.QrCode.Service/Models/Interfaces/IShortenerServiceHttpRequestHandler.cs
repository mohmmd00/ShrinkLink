namespace ShrinkLink.QrCode.Service.Models.Interfaces
{
    public interface IShortenerServiceHttpRequestHandler
    {
        Task<HttpResponseMessage> CheckCodeAsync(string code, string token, CancellationToken ct = default);
    }
}
