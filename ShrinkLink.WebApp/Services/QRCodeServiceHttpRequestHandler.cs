using System.Net;
using System.Net.Http.Headers;
using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;

namespace ShrinkLink.WebApp.Services
{
    public class QRCodeServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public QRCodeServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GenerateQrCodeAsync(QrCodeGenerateRequestTransferObject code, string token, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:5153/GenerateQrCode")
            {
                Content = JsonContent.Create(code)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request, ct);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("QR Code service is unavailable.")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unexpected error occurred.")
                };
            }
        }
    }
}