using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using System.Net;
using System.Text.Json;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class FindIpServiceHttpRequestHandler: IFindIpServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public FindIpServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostIpAddressToIpApi(string ipAddress , CancellationToken ct =default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://ip-api.com/json/{ipAddress}");
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage , ct);
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
            catch (JsonException ex)
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
