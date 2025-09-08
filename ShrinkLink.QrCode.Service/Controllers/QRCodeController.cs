using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;
using ShrinkLink.QrCode.Service.Models.Interfaces;

namespace ShrinkLink.QrCode.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IShortenerServiceHttpRequestHandler _shortenerHandler;
        private readonly IQRCodeService _qrCodeService;

        public QRCodeController(
            IShortenerServiceHttpRequestHandler shortenerHandler,
            IQRCodeService qrCodeService)
        {
            _shortenerHandler = shortenerHandler;
            _qrCodeService = qrCodeService;
        }

        [Authorize]
        [HttpPost("/GenerateQrCode")]
        public async Task<IActionResult> GenerateQrCode(
            [FromBody] QrCodeGenerateRequestTransferObject request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || !Regex.IsMatch(request.Code, "^[a-zA-Z0-9]{7}$"))
            {
                return BadRequest(new QrCodeGenerateResponseTransferObject
                {
                    Status = QrCodeGenerateStatus.InvalidInput,
                    Message = "Invalid or missing short code. Must be 7 alphanumeric characters."
                });
            }

            try
            {
                var authHeader = HttpContext.Request.Headers.Authorization.ToString();
                var shortResponse = await _shortenerHandler
                    .CheckCodeAsync(request.Code, authHeader, ct);

                if (!shortResponse.IsSuccessStatusCode)
                {
                    var content = await shortResponse.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    var message = doc.RootElement.GetProperty("message").GetString() ?? shortResponse.ReasonPhrase ?? "UnKnown";

                    return StatusCode((int)shortResponse.StatusCode,
                        new QrCodeGenerateResponseTransferObject()
                        {
                            Status = QrCodeGenerateStatus.CodeCheckingFailed,
                            Message = $"{shortResponse.ReasonPhrase}: {message}"
                        });
                }


                var linkChecker = await shortResponse.Content
                    .ReadFromJsonAsync<LinkCheckerResponseTransferObject>
                    (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);



                var qrResult = await _qrCodeService.GenerateQrCode(linkChecker.Link);

                // 5. Return appropriate HTTP result
                return qrResult.Status switch
                {
                    QrCodeGenerateStatus.Success => Ok(qrResult),
                    QrCodeGenerateStatus.InvalidInput => BadRequest(qrResult),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, qrResult)
                };
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logger
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new QrCodeGenerateResponseTransferObject
                    {
                        Status = QrCodeGenerateStatus.Failed,
                        Message = $"Unexpected error: {ex.Message}"
                    });
            }
        }
    }
}