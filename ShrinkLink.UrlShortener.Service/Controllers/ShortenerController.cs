using Microsoft.AspNetCore.Mvc;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using UAParser;
using UrlShortener.Webapp.Contracts.Dtos;

namespace ShrinkLink.UrlShortener.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly IProcessedUrlRepository _processedUrlRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly IUrlShortenerService _shortenerService;


        private readonly HttpClient _client;
        public ShortenerController(IProcessedUrlRepository processedUrlRepository, IUrlShortenerService shortenerService, IVisitorRepository visitorRepository, HttpClient client)
        {
            _processedUrlRepository = processedUrlRepository;
            _shortenerService = shortenerService;
            _visitorRepository = visitorRepository;
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

            var userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"];
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent); //ua parser !!!

            var fetchedOriginalLink = await _processedUrlRepository.FetchWantedUrl(code);
            if (string.IsNullOrEmpty(fetchedOriginalLink.OriginalUrl))
                return NotFound();
            var visit = new Visitor
                (
                code: code,
                ipAddress: userIpAddress,
                country: "iran", //must add in future
                operatingSystem: $"{clientInfo.OS.Family} {clientInfo.OS.Major}",
                browser: $"{clientInfo.UA.Family} {clientInfo.UA.Major}",

                deviceType: string.IsNullOrEmpty(clientInfo.Device.Family) || clientInfo.Device.Family == "Other"
                    ? "Unknown" : clientInfo.Device.Family,

                userAgent: Request.Headers["User-Agent"].ToString(),
                shortenGuidId: fetchedOriginalLink.Id
                );
            await _visitorRepository.CreateAsync(visit);

            return Redirect(fetchedOriginalLink.OriginalUrl);
        }


    }
}
