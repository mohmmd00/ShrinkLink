
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        
        public const int NumberOfCharsInShortLink = 7;//its not private because we need it to set a max lenght in dbcontext
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new Random();
        private readonly IShortenUrlRepository _repository;

        public UrlShortenerService(IShortenUrlRepository repository)
        {
            _repository = repository;
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

    }
}
