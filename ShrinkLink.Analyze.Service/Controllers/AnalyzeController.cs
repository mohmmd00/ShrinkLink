using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Analyze.Service.Models.Interfaces;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;
using Microsoft.AspNetCore.Authorization;

namespace ShrinkLink.Analyze.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyzeController : Controller
    {

        private readonly IShortenerServiceHttpRequestHandler _shortenerServiceHttpRequestHandler;
        private readonly IAnalyzeService _analyzeService;
        private readonly ILogger<AnalyzeController> _logger;


        public AnalyzeController(IShortenerServiceHttpRequestHandler shortenerServiceHttpRequestHandler, IAnalyzeService analyzeService, ILogger<AnalyzeController> logger)
        {
            _shortenerServiceHttpRequestHandler = shortenerServiceHttpRequestHandler;
            _analyzeService = analyzeService;
            _logger = logger;
        }


        [Authorize]
        [HttpGet("/AnalyzeProcessedLinkVisitors/{code:regex(^[[a-zA-Z0-9]]{{7}}$)}", Name = "AnalyzeProcessedLinkVisitors")]

        //:regex(^[[a-zA-Z0-9]]{{7}}$)
        public async Task<IActionResult> AnalyzeLinkAsync(
            string code,
            CancellationToken ct = default)
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized(
                    new VisitAnalysisReportResponseTransferObject
                    {
                        Status = VisitAnalysisReportStatus.Unauthorized,
                        Message = "Missing Authorization header."
                    });

            try
            {
                var shortenerServiceHttpResponseMessage = await _shortenerServiceHttpRequestHandler
                    .GetAllVisitorsByProcessedLinkAsync(code, authorizationHeader, ct);

                if (!shortenerServiceHttpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await shortenerServiceHttpResponseMessage.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    string? message = doc.RootElement.GetProperty("message").GetString() ?? shortenerServiceHttpResponseMessage.ReasonPhrase ?? "UnKnown";

                    return StatusCode((int)shortenerServiceHttpResponseMessage.StatusCode,
                        new VisitAnalysisReportResponseTransferObject()
                        {
                            Status = VisitAnalysisReportStatus.VisitorFetchingFailed,
                            Message = $"{shortenerServiceHttpResponseMessage.ReasonPhrase}: {message}"
                        });
                }

                var shortenerResponseAsObject = await shortenerServiceHttpResponseMessage.Content
                    .ReadFromJsonAsync<VisitsResponseTransferObject>
                    (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

                if (shortenerResponseAsObject is null)
                {
                    return BadRequest(
                        new VisitAnalysisReportResponseTransferObject
                        {
                            Status = VisitAnalysisReportStatus.Failed,
                            Message = shortenerResponseAsObject?.Message ?? "Failed to retrieve visitor data."
                        });
                }

                if (!shortenerResponseAsObject.VisitorsAsData.Any())
                {
                    return Ok(
                        new VisitAnalysisReportResponseTransferObject
                        {
                            Status = VisitAnalysisReportStatus.VisitorNotFound,
                            Message = shortenerResponseAsObject?.Message ?? "No visitor data available."
                        });
                }

                var analysisResult = await _analyzeService.AnalyzeVisitsAsync(shortenerResponseAsObject.VisitorsAsData, ct);

                return analysisResult.Status switch
                {
                    VisitAnalysisReportStatus.Success => Ok(analysisResult),
                    VisitAnalysisReportStatus.InvalidData => BadRequest(analysisResult),
                    VisitAnalysisReportStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, analysisResult),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, analysisResult)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Shortener service unreachable.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new VisitAnalysisReportResponseTransferObject
                    {
                        Status = VisitAnalysisReportStatus.Failed,
                        Message = "Shortener service is unavailable."
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                return Problem(
                    title: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize]
        [HttpGet("/AnalyzeUserVisitors", Name = "AnalyzeUserVisitors")]
        public async Task<IActionResult> AnalyzeUserAsync(CancellationToken ct = default)
        {

            var authorizationHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized(new VisitAnalysisReportResponseTransferObject
                {
                    Status = VisitAnalysisReportStatus.Unauthorized,
                    Message = "Missing Authorization header."
                });
            try
            {
                var shortenerServiceHttpResponseMessage = await _shortenerServiceHttpRequestHandler
                    .GetAllVisitorsByUserIdAsync(authorizationHeader, ct);

                if (!shortenerServiceHttpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await shortenerServiceHttpResponseMessage.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    string? message = doc.RootElement.GetProperty("message").GetString() ?? shortenerServiceHttpResponseMessage.ReasonPhrase ?? "UnKnown";

                    return StatusCode((int)shortenerServiceHttpResponseMessage.StatusCode,
                        new VisitAnalysisReportResponseTransferObject()
                        {
                            Status = VisitAnalysisReportStatus.VisitorFetchingFailed,
                            Message = $"{shortenerServiceHttpResponseMessage.ReasonPhrase}: {message}"
                        });
                }


                var shortenerResponseAsObject = await shortenerServiceHttpResponseMessage.Content
                    .ReadFromJsonAsync<VisitsResponseTransferObject>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);


                if (shortenerResponseAsObject.VisitorsAsData == null)
                {
                    return StatusCode((int)shortenerServiceHttpResponseMessage.StatusCode,
                        new VisitAnalysisReportResponseTransferObject
                        {
                            Status = VisitAnalysisReportStatus.Failed,
                            Message = shortenerResponseAsObject?.Message ?? "Failed to retrieve visitor data."
                        });
                }
                if (!shortenerResponseAsObject.VisitorsAsData.Any())
                {
                    return Ok(new VisitAnalysisReportResponseTransferObject()
                    {
                        Status = VisitAnalysisReportStatus.VisitorNotFound,
                        Message = shortenerResponseAsObject?.Message ?? "No visitor data available."


                    });
                }
                var analysisReport = await _analyzeService.AnalyzeVisitsAsync(shortenerResponseAsObject.VisitorsAsData, ct);
                return analysisReport.Status switch
                {
                    VisitAnalysisReportStatus.Success => Ok(analysisReport),
                    VisitAnalysisReportStatus.InvalidData => BadRequest(analysisReport),
                    VisitAnalysisReportStatus.Failed => StatusCode(StatusCodes.Status500InternalServerError, analysisReport),
                    VisitAnalysisReportStatus.VisitorNotFound => Ok(analysisReport)
                };

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Shortener service unreachable.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new VisitAnalysisReportResponseTransferObject
                    {
                        Status = VisitAnalysisReportStatus.Failed,
                        Message = "Shortener service is unavailable."
                    });
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
