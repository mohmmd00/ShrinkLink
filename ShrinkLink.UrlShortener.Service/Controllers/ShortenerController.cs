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
        private readonly IShortenUrlRepository _shortenUrlRepository;
        private readonly IUrlShortenerService _shortenerService;
        private readonly IVisitorRepository _visitorRepository;
        public ShortenerController(IShortenUrlRepository shortenUrlRepository, IUrlShortenerService shortenerService, IVisitorRepository visitorRepository)
        {
            _shortenUrlRepository = shortenUrlRepository;
            _shortenerService = shortenerService;
            _visitorRepository = visitorRepository;
        }
        [HttpPost(Name = "ShortenLink")]
        public async Task<IActionResult> PostLinkAsync(LongLinkTransferObject longUrl)
        {
            if (!Uri.TryCreate(longUrl.LongLink, UriKind.Absolute, out _))
                return BadRequest("Invalid URL format.");

            var newUniqueCode = await _shortenerService.GenerateUniqueCodeAsync();
            var shortenedUrl = new ShortenUrl
                (longUrl.LongLink, newUniqueCode, $"https://{Request.Host}/{newUniqueCode}");
            
            await _shortenUrlRepository.CreateAsync(shortenedUrl);

            var obj = new ShortLinkTransferObject{ShortUrl = shortenedUrl.ShortUrl };
            return Ok(obj);
        }
        [HttpGet("/{code}", Name = "Redirect")]
        public async Task<IActionResult> RedirectToOriginalUrl(string code)
        {

            var userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"];
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent); //ua parser !!!

            var fetchedOriginalLink = await _shortenUrlRepository.FetchWantedUrl(code);
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
                shortenGuidId:fetchedOriginalLink.Id
                );
            _visitorRepository.CreateAsync(visit);

            return Redirect(fetchedOriginalLink.OriginalUrl);
        }


    }
}
