using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {

        public const int NumberOfCharsInShortLink = 7;//its not private because we need it to set a max lenght in dbcontext
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new Random();
        private readonly IProcessedUrlRepository _repository;
        private readonly IConfiguration _configuration;

        public UrlShortenerService(IProcessedUrlRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<LinkShortenerResponseTransferObject> GenerateShortLink(LinkShortenerRequestTransferObject request, Guid userPrimaryId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.LongLink) ||
                !Uri.TryCreate(request.LongLink, UriKind.Absolute, out _))
            {
                return new LinkShortenerResponseTransferObject
                {
                    Message = "The provided URL is invalid.",
                    Status = LinkShortenerResultStatus.InvalidUrl
                };
            }
            try
            {
                var uniqueCode = await GenerateUniqueCodeAsync(ct);

                var processedUrl = new ProcessedUrl(
                    originalUrl: request.LongLink,
                    code: uniqueCode,
                    userId: userPrimaryId
                );

                bool result = await _repository.CreateAsync(processedUrl, ct);

                var address = _configuration["ApiSettings:ShrinkAddress"];
                if (result)
                {
                    return new LinkShortenerResponseTransferObject
                    {
                        ShortUrl = $"{address}/{uniqueCode}",
                        Message = "Short URL generated successfully.",
                        Status = LinkShortenerResultStatus.Success
                    };
                }

                return new LinkShortenerResponseTransferObject()
                {
                    Message = "Unable to create a new processed Link",
                    Status = LinkShortenerResultStatus.ShortLinkCreationFailed
                };

            }
            catch (Exception ex)
            {
                return new LinkShortenerResponseTransferObject
                {
                    Message = $"Unexpected error: {ex.Message}",
                    Status = LinkShortenerResultStatus.Failed
                };
            }
        }

        public async Task<AllLinksFetcherResponseTransferObject> FetchAllProcessedLinks(Guid userPrimaryId, CancellationToken ct = default)
        {
            try
            {
                var links = await _repository.FetchAllProcessedUrlsByUserId(userPrimaryId, ct);

                if (links == null)
                {
                    return new AllLinksFetcherResponseTransferObject()
                    {
                        Status = AllLinksFetcherResultStatus.Failed,
                        Message = "Error: No data returned from repository."
                    };
                }

                if (!links.Any())
                {
                    return new AllLinksFetcherResponseTransferObject
                    {
                        Message = "There is no Links to fetch",
                        Status = AllLinksFetcherResultStatus.NoLinksFound,
                        LinksToViewModel = new List<LinksViewModel>()
                    };
                }


                //binding processed links to a viewModel
                var linksViewModel = links.Select(l => new LinksViewModel
                {
                    OriginalUrl = l.OriginalUrl,
                    Code = l.Code,
                    CreatedAt = l.CreatedAt,
                }).ToList();

                return new AllLinksFetcherResponseTransferObject
                {
                    Message = "Fetching was successful",
                    Status = AllLinksFetcherResultStatus.Success,
                    LinksToViewModel = linksViewModel
                };
            }
            catch (Exception e)
            {
                return new AllLinksFetcherResponseTransferObject()
                {
                    Status = AllLinksFetcherResultStatus.Failed,
                    Message = $"Unexpected error: {e.Message}"
                };
            }
        }
        public async Task<LinkRemoverResponseTransferObject> RemoveSelectedLink(Guid userPrimaryId, string shortenedLinkCode, CancellationToken ct = default)
        {

            if (string.IsNullOrEmpty(shortenedLinkCode))
            {
                return new LinkRemoverResponseTransferObject()
                {
                    Status = LinkRemoverResultStatus.InvalidCode,
                    Message = "The provided Code is invalid."
                };
            }

            try
            {
                bool result = await _repository.DeleteAsync(userPrimaryId, shortenedLinkCode, ct);
                if (result)
                {
                    return new LinkRemoverResponseTransferObject()
                    {
                        Status = LinkRemoverResultStatus.Success,
                        Message = "ProcessedLink Removed Successfully"
                    };
                }
                return new LinkRemoverResponseTransferObject()
                {
                    Status = LinkRemoverResultStatus.InvalidInformation,
                    Message = "Unable to Remove ProcessedLink"
                };
            }
            catch (Exception e)
            {
                return new LinkRemoverResponseTransferObject()
                {
                    Status = LinkRemoverResultStatus.Failed,
                    Message = $"Unexpected error: {e.Message}"
                };
            }

        }
        public async Task<LinkCheckerResponseTransferObject> CheckSelectedLink(Guid userPrimaryId, string code, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(code))
            {
                return new LinkCheckerResponseTransferObject()
                {
                    Status = LinkCheckerResultStatus.InvalidCode,
                    Message = "The provided Code is invalid."
                };
            }

            try
            {
                var link = await _repository.FetchWantedUrl(code, ct);

                if (link == null || link.UserId != userPrimaryId)
                {
                    return new LinkCheckerResponseTransferObject()
                    {
                        Status = LinkCheckerResultStatus.InvalidCode,
                        Message = "The specified code does not exist or does not belong to the user."
                    };
                }
                var shrinkLinkAddress = _configuration["ApiSettings:ShrinkAddress"];
                return new LinkCheckerResponseTransferObject()
                {
                    Status = LinkCheckerResultStatus.Success,
                    Link = $"{shrinkLinkAddress}/{code}",
                    Message = "Link found successfully.",
                };
            }
            catch (Exception e)
            {
                return new LinkCheckerResponseTransferObject()
                {
                    Status = LinkCheckerResultStatus.Failed,
                    Message = $"Unexpected error: {e.Message}"
                };
            }
        }
        private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default)
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
                bool isCodeExist = await _repository.IsCodeExistsAsync(code, ct);//temp must change that later

                if (!isCodeExist)
                {
                    return code;
                }
            }
        }

    }
}
