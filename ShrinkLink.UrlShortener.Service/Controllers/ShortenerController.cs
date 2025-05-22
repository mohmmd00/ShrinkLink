using Microsoft.AspNetCore.Mvc;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using UrlShortener.Webapp.Contracts.Dtos;

namespace ShrinkLink.UrlShortener.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController : ControllerBase
    {

        private readonly HttpClient _client;

        private readonly IHttpUserAgentParserProvider _httpUserAgentParser;

        private readonly IProcessedUrlRepository _processedUrlRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly IUrlShortenerService _shortenerService;

        public ShortenerController(IProcessedUrlRepository processedUrlRepository, IUrlShortenerService shortenerService, IVisitorRepository visitorRepository, HttpClient client , IHttpUserAgentParserProvider httpUserAgentParser)
        {
            _processedUrlRepository = processedUrlRepository;
            _shortenerService = shortenerService;
            _visitorRepository = visitorRepository;
            _httpUserAgentParser = httpUserAgentParser;
            _client = client;
        }
        [HttpPost(Name = "ShortenLink")]
        public async Task<IActionResult> PostLinkAsync(LongLinkTransferObject longUrl)
        {
            if (!Uri.TryCreate(longUrl.LongLink, UriKind.Absolute, out _))
                return BadRequest("Invalid URL format.");
            var token = Request.Cookies["jwt-token"];


            Console.WriteLine($"the token is : {token}");

            var newUniqueCode = await _shortenerService.GenerateUniqueCodeAsync();
            var userinfo = await _shortenerService.GetUserBaseInformationAsync(token);
            if (userinfo != null)
            {

                var shortenedUrl = new ProcessedUrl
                    (longUrl.LongLink, newUniqueCode, userinfo.Id);

                await _processedUrlRepository.CreateAsync(shortenedUrl);


                var obj = new ShortLinkTransferObject { ShortUrl = $"https://{Request.Host}/{newUniqueCode}" };
                return Ok(obj);
            }

            return NotFound("user not found");
        }
        [HttpGet("/{code}", Name = "Redirect")]
        public async Task<IActionResult> RedirectToOriginalUrl(string code)
        {
            var fetchedOriginalLink = await _processedUrlRepository.FetchWantedUrl(code);

            if (!string.IsNullOrEmpty(fetchedOriginalLink.OriginalUrl))
            {
                var userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgentInformation = _httpUserAgentParser.Parse(Request.Headers["User-Agent"]);

                string deviceType = userAgentInformation.IsMobile() ? "Mobile" : "Desktop";

                var newVisit = new Visitor
                (
                    processedUrlCode: code,
                    ipAddress: userIpAddress,
                    country: "Iran",
                    operatingSystem: userAgentInformation.Platform.Value.Name,
                    browser: userAgentInformation.Name,
                    deviceType: deviceType,
                    userAgent: userAgentInformation.UserAgent,
                    shortenGuidId: fetchedOriginalLink.Id
                );
                await _visitorRepository.CreateAsync(newVisit);
                return Redirect(fetchedOriginalLink.OriginalUrl);
            }

            return NotFound();


        }


    }
}
