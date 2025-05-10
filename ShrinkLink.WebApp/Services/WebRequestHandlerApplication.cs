using System.Text.Json;
using UrlShortener.Webapp.Contracts.Dtos;

namespace ShrinkLink.WebApp.Services
{
    public class WebRequestHandlerApplication
    {
        private readonly HttpClient _httpClient;

        public WebRequestHandlerApplication(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ShortLinkTransferObject?> PostLinkHandlerAsync(LongLinkTransferObject model)
        {
            Console.WriteLine(model.LongLink);
            var longlink = new LongLinkTransferObject { LongLink = model.LongLink };


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

    }
}
