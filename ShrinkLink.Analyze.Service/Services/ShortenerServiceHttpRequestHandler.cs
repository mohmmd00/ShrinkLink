using System.Net;
using System.Net.Http.Headers;
using ShrinkLink.Analyze.Service.Models.Interfaces;

namespace ShrinkLink.Analyze.Service.Services
{
    public class ShortenerServiceHttpRequestHandler : IShortenerServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public ShortenerServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAllVisitorsByProcessedLinkAsync(string code, string token, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://localhost:5062/VisitorsByLinkCode/{code}");
            var cleanedUpToken = token.StartsWith("Bearer ")
                ? token.Substring("Bearer ".Length).Trim()
                : token;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cleanedUpToken);

            try
            {
                var response = await _httpClient.SendAsync(requestMessage, ct);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("The service is currently unreachable.")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unexpected error occurred.")
                };
            }
        }

        public async Task<HttpResponseMessage> GetAllVisitorsByUserIdAsync(string token, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://localhost:5062/VisitorsByUserId");
            var cleanedUpToken = token.StartsWith("Bearer ") ? token.Substring("Bearer ".Length).Trim() : token;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cleanedUpToken);

            try
            {
                var response = await _httpClient.SendAsync(requestMessage, ct);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("The service is currently unreachable.")
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
