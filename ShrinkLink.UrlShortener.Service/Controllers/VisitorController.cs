using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;
using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.UrlShortener.Service.Models.DataTransferObjects;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ShrinkLink.UrlShortener.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly IHttpUserAgentParserProvider _httpUserAgentParser;
        private readonly IFindIpServiceHttpRequestHandler _findIpServiceHttpRequestHandler;
        private readonly IProcessedUrlRepository _processedUrlRepository;
        private readonly IVisitorService _visitorService;
        private readonly IVisitorRepository _visitorRepository;

        public VisitorController(IHttpUserAgentParserProvider httpUserAgentParser,
            IFindIpServiceHttpRequestHandler findIpServiceHttpRequestHandler,
            IProcessedUrlRepository processedUrlRepository,
            IVisitorService visitorService,
            IVisitorRepository visitorRepository)
        {
            _httpUserAgentParser = httpUserAgentParser;
            _findIpServiceHttpRequestHandler = findIpServiceHttpRequestHandler;
            _processedUrlRepository = processedUrlRepository;
            _visitorService = visitorService;
            _visitorRepository = visitorRepository;
        }

        [HttpGet("/{code}", Name = "Redirect")]
        public async Task<IActionResult> RedirectToOriginalUrl(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { Error = "Invalid short code." });
            }

            try
            {
                var fetchedOriginalLink = await _processedUrlRepository.FetchWantedUrl(code, ct);
                if (fetchedOriginalLink == null || string.IsNullOrEmpty(fetchedOriginalLink.OriginalUrl))
                {
                    return NotFound(new { Message = "The requested URL could not be found." });
                }

                var userIpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.ToString();

                Random random = new Random();
                var randomIpAddress = $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";


                var httpResponseMessage = await _findIpServiceHttpRequestHandler.PostIpAddressToIpApi(randomIpAddress, ct);
                var ipAddressInformation = httpResponseMessage.IsSuccessStatusCode
                    ? await httpResponseMessage.Content.ReadFromJsonAsync<IpApiResponse>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct)
                    : new IpApiResponse { Country = "Unknown", City = "Unknown" };

                var userAgentHeader = Request.Headers["User-Agent"].ToString();
                var userAgentInfo = _httpUserAgentParser.Parse(userAgentHeader);
                string deviceType = userAgentInfo.IsMobile() ? "Mobile" : "Desktop";

                var newVisit = new Visitor(
                    processedUrlCode: code,
                    ipAddress: userIpAddress,
                    country: ipAddressInformation.Country,
                    operatingSystem: userAgentInfo.Platform.Value.Name,
                    browser: userAgentInfo.Name ?? "Unknown",
                    deviceType: deviceType,
                    userAgent: userAgentHeader,
                    shortenGuidId: fetchedOriginalLink.Id,
                    city: ipAddressInformation.City
                );

                await _visitorRepository.CreateAsync(newVisit, ct);
                return Redirect(fetchedOriginalLink.OriginalUrl);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "An unexpected error occurred while processing your request.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        [Authorize]
        [HttpGet("/VisitorsByLinkCode/{code:regex(^[[a-zA-Z0-9]]{{7}}$)}", Name = "VisitorsByLinkCode")]
        public async Task<IActionResult> GetAllVisitorsByLinkCodeAsync(string code, CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }

            try
            {
                var Response = await _visitorService.FetchAllVisitsByLinkCodeAsync(userId, code, ct);
                return Response.Status switch
                {
                    VisitorsByCodeResultStatus.Success => Ok(Response),
                    VisitorsByCodeResultStatus.VisitorNotFound => Ok(Response),
                    VisitorsByCodeResultStatus.InvalidCode => BadRequest(Response),
                    VisitorsByCodeResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, new { Response.Message }),
                    _ => BadRequest("Invalid response status.")
                };
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "An unexpected error occurred.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("/VisitorsByUserId", Name = "VisitorsByUserId")]
        public async Task<IActionResult> GetAllVisitorsByUserId(CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }

            try
            {
                var Response = await _visitorService.FetchAllVisitsByUserIdAsync(userId, ct);
                return Response.Status switch
                {
                    VisitorsByCodeResultStatus.Success => Ok(Response),
                    VisitorsByCodeResultStatus.VisitorNotFound => Ok(Response),
                    VisitorsByCodeResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, new { Response.Message }),
                };

            }
            catch (Exception ex)
            {
                return Problem(
                    title: "An unexpected error occurred.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


    }
}