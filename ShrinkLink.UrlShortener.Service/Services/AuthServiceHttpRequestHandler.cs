using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using System.Net;
using System.Net.Http.Headers;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class AuthServiceHttpRequestHandler : IAuthServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public AuthServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetUserBaseInformationAsync(string token, CancellationToken ct = default)
        {
            //var response = await _httpClient.PostAsJsonAsync("api/Auth/UserBaseDetails", token);
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://localhost:5054/UserBasicDetails");

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
    }
}
