
using System.Net;
using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;

namespace ShrinkLink.WebApp.Services
{
    public class AuthServiceHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public AuthServiceHttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostRegisterRequest(RegisterRequestTransferObject request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5054/Register")
            {
                Content = JsonContent.Create(request)
            };

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error during registration: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("The service is currently unreachable.")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error during registration: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unexpected error occurred.")
                };
            }
        }
        public async Task<HttpResponseMessage> PostLoginRequest(LoginRequestTransferObject request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5054/Login")
            {
                Content = JsonContent.Create(request)
            };

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error during login: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("The service is currently unreachable.")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error during login: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unexpected error occurred.")
                };
            }
        }
    }
}