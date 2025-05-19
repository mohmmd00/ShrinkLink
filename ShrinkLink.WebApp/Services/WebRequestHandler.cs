using System.Net.Http.Headers;
using System.Text.Json;
using AuthService.Webapp.Contracts.Dtos;
using UrlShortener.Webapp.Contracts.Dtos;

namespace ShrinkLink.WebApp.Services
{
    public class WebRequestHandler
    {
        private readonly HttpClient _httpClient;

        public WebRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ShortLinkTransferObject?> PostLinkHandlerAsync(LongLinkTransferObject request , string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var longlink = new LongLinkTransferObject { LongLink = request.LongLink }; // :|||| wtf is this 


            var response = await _httpClient.PostAsJsonAsync("api/Shortener", longlink);

            Console.WriteLine(response.Content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // You can log this or throw an exception, depending on your needs
                Console.WriteLine("API error: " + errorMessage);
                return null;
            }

            var responseModel = await response.Content.ReadFromJsonAsync<ShortLinkTransferObject>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return responseModel;
        }





        public async Task<RegisterResponseTransferObject> PostRegisterRequest(RegisterTransferObject request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // You can log this or throw an exception, depending on your needs
                Console.WriteLine("API error: " + errorMessage);
                return null;
            }
            var responseModel = await response.Content.ReadFromJsonAsync<RegisterResponseTransferObject>
                (new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return responseModel;
        }


        public async Task<LoginResponseTransferObject> PostLoginRequest(LoginTransferObject request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // You can log this or throw an exception, depending on your needs
                Console.WriteLine("API error: " + errorMessage);
                return null;
            }

            var responseModel = await response.Content.ReadFromJsonAsync<LoginResponseTransferObject>
            (new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return responseModel;
        }

    }
}
