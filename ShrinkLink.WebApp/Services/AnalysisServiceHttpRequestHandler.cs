using System.Net;
using System.Net.Http.Headers;

namespace ShrinkLink.WebApp.Services
{
    public class AnalysisServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public AnalysisServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<HttpResponseMessage> ReceiveAnalysisOfUsersAllVisitsAsync(string token, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: "http://localhost:5013/AnalyzeUserVisitors");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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



        public async Task<HttpResponseMessage> ReceiveAnalysisOfSingleCodeAsync(string code, string token, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://localhost:5013/AnalyzeProcessedLinkVisitors/{code}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
