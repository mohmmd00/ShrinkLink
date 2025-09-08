using System.Net;
using System.Net.Http.Headers;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using static System.Net.WebRequestMethods;

namespace ShrinkLink.WebApp.Services
{

    //this handler is for sending http request to shortener service api

    public class ShortenerServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public ShortenerServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<HttpResponseMessage> PostLongLinkAsync(LinkShortenerRequestTransferObject request
            , string token, CancellationToken ct = default)
        {
            using var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:5062/Shorten")
            {
                Content = JsonContent.Create(request)
            };

            //requestMessage.Headers.Add("Authorization", $"Bearer {token}"); works but its not standard
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(requestMessage, ct);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error: " + ex.Message);

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


        public async Task<HttpResponseMessage> RemoveLink(string token, string code, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"http://localhost:5062/RemoveSelectedLinkByCode/{code}");

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
        public async Task<HttpResponseMessage> GetAllProcessedLinksByUserIdAsync(string token, CancellationToken ct = default)
        {

            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: "http://localhost:5062/FetchAllLinks");

            //requestMessage.Headers.Add("Authorization", $"Bearer {token}"); //works but its not standard
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

        public async Task<HttpResponseMessage> GetLinkVisitCountsByLinkCodeAsync(string token,
            string code, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(
                method: HttpMethod.Post,
                requestUri: "http://localhost:5062/VisitCountByLinkCode");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(requestMessage, ct);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error: " + ex.Message);

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
