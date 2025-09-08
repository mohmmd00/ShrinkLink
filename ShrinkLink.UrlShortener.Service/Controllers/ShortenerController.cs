using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController : ControllerBase
    {


        private readonly IUrlShortenerService _shortenerService;

        public ShortenerController(IUrlShortenerService shortenerService, IAuthServiceHttpRequestHandler authServiceHttpRequestHandler)
        {
            _shortenerService = shortenerService;
        }



        [Authorize]
        [HttpPost($"/Shorten", Name = "ShortenLink")]
        public async Task<IActionResult> ShortenLinkAsync([FromBody] LinkShortenerRequestTransferObject request,
            CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new LinkShortenerResponseTransferObject()
                {
                    Status = LinkShortenerResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }

            try
            {
                var response = await _shortenerService.GenerateShortLink(request, userId, ct);
                return response.Status switch
                {
                    LinkShortenerResultStatus.Success => Ok(response),
                    LinkShortenerResultStatus.InvalidUrl => BadRequest(response),
                    LinkShortenerResultStatus.ShortLinkCreationFailed => Conflict(response),
                    LinkShortenerResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, response)
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
        [HttpGet("/FetchAllLinks")]
        public async Task<IActionResult> FetchProcessedLinks(CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new AllLinksFetcherResponseTransferObject()
                {
                    Status = AllLinksFetcherResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }
            try
            {
                var processedLinkResponse = await _shortenerService.FetchAllProcessedLinks(userId, ct);
                return processedLinkResponse?.Status switch
                {
                    AllLinksFetcherResultStatus.Success => Ok(processedLinkResponse),
                    AllLinksFetcherResultStatus.NoLinksFound => Ok(processedLinkResponse),
                    AllLinksFetcherResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, processedLinkResponse)

                };

            }
            catch (Exception ex)
            {
                return Problem(title: "An unexpected error occurred",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize]
        [HttpGet("/CheckProcessedLink/{code}", Name = "LinkChecker")]
        public async Task<IActionResult> CheckLinkByCode(string code, CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new LinkCheckerResponseTransferObject()
                {
                    Status = LinkCheckerResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }
            try
            {
                var linkCheckerResponse = await _shortenerService.CheckSelectedLink(userId, code, ct);
                return linkCheckerResponse.Status switch
                {
                    LinkCheckerResultStatus.Success => Ok(linkCheckerResponse),
                    LinkCheckerResultStatus.InvalidCode => BadRequest(linkCheckerResponse),
                    LinkCheckerResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, linkCheckerResponse),
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        [Authorize]
        [HttpGet("/RemoveSelectedLinkByCode/{code}", Name = "LinkRemover")]
        public async Task<IActionResult> RemoveLinkByCode(string code, CancellationToken ct = default)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(value, out var userId))
            {
                return Unauthorized(new LinkRemoverResponseTransferObject()
                {
                    Status = LinkRemoverResultStatus.Unauthorized,
                    Message = "Cannot parse user identifier."
                });
            }
            try
            {
                var linkRemoverResponse = await _shortenerService.RemoveSelectedLink(userId, code, ct);
                return linkRemoverResponse.Status switch
                {
                    LinkRemoverResultStatus.Success => Ok(linkRemoverResponse),
                    LinkRemoverResultStatus.InvalidCode => BadRequest(linkRemoverResponse),
                    LinkRemoverResultStatus.InvalidInformation => BadRequest(linkRemoverResponse), //must remove 
                    LinkRemoverResultStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, linkRemoverResponse),

                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }
    }
}
