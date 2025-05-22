
using System.Text.Json;
using AuthService.UrlShortener.Contracts.Dtos;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {

        public const int NumberOfCharsInShortLink = 7;//its not private because we need it to set a max lenght in dbcontext
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new Random();
        private readonly IProcessedUrlRepository _repository;
        private readonly HttpClient _httpClient;

        public UrlShortenerService(IProcessedUrlRepository repository, HttpClient httpClient)
        {
            _repository = repository;
            _httpClient = httpClient;
        }

        public async Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default)
        {
            var chars = new char[NumberOfCharsInShortLink];

            while (true)
            {
                for (int i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    int randomIndex = _random.Next(Alphabet.Length - 1);

                    chars[i] = Alphabet[randomIndex];
                }

                var code = new string(chars);
                bool IsCodeExist = await _repository.IsCodeExistsAsync(code, ct);

                if (!IsCodeExist)
                {
                    return code;
                }
            }
        }

        public async Task<UserBasicInformation> GetUserBaseInformationAsync(string token, CancellationToken ct = default)
        {
            //var response = await _httpClient.PostAsJsonAsync("api/Auth/UserBaseDetails", token);
            var response2 = await _httpClient.GetFromJsonAsync<UserBasicInformation>($"https://localhost:7030/api/Auth/UserBaseDetails/{token}");


            //if (response2 == null)
            //{
            //    var errorMessage = await response2.Content.ReadAsStringAsync();
            //    // You can log this or throw an exception, depending on your needs
            //    Console.WriteLine("API error: " + errorMessage);
            //    return null;
            //}

            //var responseModel = await response.Content.ReadFromJsonAsync<UserBasicInformation>(new JsonSerializerOptions
            //{ PropertyNameCaseInsensitive = true });

            return response2;

        }

    }
}
