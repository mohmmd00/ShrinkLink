using Microsoft.AspNetCore.Mvc;
using ShrinkLink.WebApp.Services;
using System.Text.Json;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.WebApp.Models.DashBoardBindingModels;
using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;
using ShrinkLink.WebApp.attribute;
using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;
using System.Net;


namespace ShrinkLink.WebApp.Controllers
{
    [JwtAuthorize]
    public class DashboardController : Controller
    {
        private readonly ShortenerServiceHttpRequestHandler _shortenerServiceHttpRequestHandler;
        private readonly AnalysisServiceHttpRequestHandler _analysisServiceHttpRequestHandler;
        private readonly QRCodeServiceHttpRequestHandler _qrCodeServiceHttpRequestHandler;

        public DashboardController(ShortenerServiceHttpRequestHandler shortenerServiceHttpRequestHandler, AnalysisServiceHttpRequestHandler analysisServiceHttpRequestHandler, QRCodeServiceHttpRequestHandler qrCodeServiceHttpRequestHandler)
        {
            _shortenerServiceHttpRequestHandler = shortenerServiceHttpRequestHandler;
            _analysisServiceHttpRequestHandler = analysisServiceHttpRequestHandler;
            _qrCodeServiceHttpRequestHandler = qrCodeServiceHttpRequestHandler;
        }

        public IActionResult Index()
        {
            return View("Index");
        }


        //Analysis functions -------------------------------------------
        public async Task<IActionResult> Analyze(CancellationToken ct = default)
        {
            var authorizationCookie = Request.Cookies["Authorization"];
            if (string.IsNullOrEmpty(authorizationCookie))
            {
                TempData["AnalysisVisitsErrorMessage"] = "Unauthorized access.";
                TempData["FetchAllProcessLinksErrorMessage"] = "Unauthorized access.";
                return View("Analyze");
            }
            try
            {
                var shortenerHttpResponseMessage = await _shortenerServiceHttpRequestHandler
                    .GetAllProcessedLinksByUserIdAsync(authorizationCookie, ct);
                var analysisHttpResponseMessage = await _analysisServiceHttpRequestHandler
                    .ReceiveAnalysisOfUsersAllVisitsAsync(authorizationCookie, ct);


                if (!shortenerHttpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await shortenerHttpResponseMessage.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    string? message = doc.RootElement.GetProperty("message").GetString() ?? shortenerHttpResponseMessage.ReasonPhrase ?? "UnKnown";

                    TempData["FetchAllProcessLinksErrorMessage"] =
                        $"{shortenerHttpResponseMessage.ReasonPhrase}: {message}";
                }
                if (!analysisHttpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await analysisHttpResponseMessage.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    string? message = doc.RootElement.GetProperty("message").GetString() ?? analysisHttpResponseMessage.ReasonPhrase ?? "UnKnown";
                    TempData["AnalysisVisitsErrorMessage"] = $"{analysisHttpResponseMessage.ReasonPhrase}: {message}";
                }


                var shortenerResponseMessageAsObject = await shortenerHttpResponseMessage
                    .Content.ReadFromJsonAsync<AllLinksFetcherResponseTransferObject>
                        (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

                var analysisResponseMessageAsObject = await analysisHttpResponseMessage.Content
                        .ReadFromJsonAsync<VisitAnalysisReportResponseTransferObject>
                        (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);



                var analysisModel = new AnalyzeModel()
                {
                    Links = shortenerResponseMessageAsObject,
                    AnalyzeViewModel = analysisResponseMessageAsObject
                };

                return View("Analyze", analysisModel);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse JSON response from analysis API: {ex.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View("Analyze");
        }
        public async Task<IActionResult> AnalyzeLink(string code, CancellationToken ct = default)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];
                if (string.IsNullOrEmpty(authorizationCookie))
                {
                    TempData["AnalysisVisitsErrorMessage"] = "Unauthorized access.";
                    return View("AnalyzeSingleLink");
                }
                var analysisHttpResponseMessage =
                    await _analysisServiceHttpRequestHandler.ReceiveAnalysisOfSingleCodeAsync(code, authorizationCookie, ct);

                if (!analysisHttpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await analysisHttpResponseMessage.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    string? message = doc.RootElement.GetProperty("message").GetString() ?? analysisHttpResponseMessage.ReasonPhrase ?? "UnKnown";
                    TempData["AnalysisVisitsErrorMessage"] = $"{analysisHttpResponseMessage.ReasonPhrase}: {message}";
                    return View("AnalyzeSingleLink");
                }



                var analysisResponseAsObject =
                    await analysisHttpResponseMessage.Content
                        .ReadFromJsonAsync<VisitAnalysisReportResponseTransferObject>
                        (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

                return View("AnalyzeSingleLink",
                    new AnalyzeSingleLinkModel
                    {
                        AnalyzeViewModel = analysisResponseAsObject
                    });

            }
            catch (JsonException ex)
            {
                // JSON parsing error
                TempData["AnalysisVisitsErrorMessage"] = "Invalid response format from analysis service.";
            }
            catch (HttpRequestException ex)
            {
                // Network/communication errors
                TempData["AnalysisVisitsErrorMessage"] = "Failed to connect to analysis service. Please check your connection.";
            }

            return View("AnalyzeSingleLink");
        }
        //link shortener functions ------------------------------------
        public async Task<IActionResult> Links(CancellationToken ct = default)
        {
            var authorizationCookie = Request.Cookies["Authorization"];
            if (string.IsNullOrEmpty(authorizationCookie))
            {
                TempData["AnalysisVisitsErrorMessage"] = "Unauthorized access.";
                TempData["FetchAllProcessLinksErrorMessage"] = "Unauthorized access.";
                return View("Analyze");
            }
            //fetch all processed links by user and show them 
            try
            {
                var receiveAllLinksHttpResponse =
                    await _shortenerServiceHttpRequestHandler
                        .GetAllProcessedLinksByUserIdAsync(authorizationCookie, ct);


                //if (receiveAllLinksHttpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                //{
                //    TempData["FetchAllProcessLinksErrorMessage"] = $"{receiveAllLinksHttpResponse.Content}";
                //    return View("Index");

                //}

                if (!receiveAllLinksHttpResponse.IsSuccessStatusCode)
                {
                    var content = await receiveAllLinksHttpResponse.Content.ReadAsStringAsync(ct);
                    using var doc = JsonDocument.Parse(content);
                    var message = doc.RootElement.GetProperty("message").GetString() ??
                                  receiveAllLinksHttpResponse.ReasonPhrase ?? "UnKnown";

                    TempData["FetchAllProcessLinksErrorMessage"] =
                        $"{receiveAllLinksHttpResponse.ReasonPhrase}: {message}";

                }
                else
                {

                    var receiveAllLinksResponseConvertedToObject = await receiveAllLinksHttpResponse.Content
                        .ReadFromJsonAsync<AllLinksFetcherResponseTransferObject>
                            (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

                    var result = new LinkModel
                    {
                        LinksViewModel = receiveAllLinksResponseConvertedToObject
                    };

                    return View("Links", result);
                }



            }
            catch (JsonException ex)
            {
                // Log JSON parsing error
                Console.WriteLine($"Failed to parse JSON response: {ex.Message}");
            }

            return View("Links");
        }
        public async Task<IActionResult> ShortenerAsync(LinkModel request, CancellationToken ct = default)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];
                var shortenerLinkHttpResponse =
                    await _shortenerServiceHttpRequestHandler.PostLongLinkAsync(request.LonglinkModel, authorizationCookie);
                if (shortenerLinkHttpResponse.IsSuccessStatusCode)
                {
                    var shortenerLinkResponseAsObject = await shortenerLinkHttpResponse
                        .Content.ReadFromJsonAsync<LinkShortenerResponseTransferObject>(new JsonSerializerOptions
                        { PropertyNameCaseInsensitive = true });

                    if (shortenerLinkResponseAsObject.Status != LinkShortenerResultStatus.Success)
                    {
                        TempData["ProcessLinkErrorMessage"] = shortenerLinkResponseAsObject.Message;
                    }
                    else
                    {
                        TempData["LongLink"] = request.LonglinkModel.LongLink;
                        TempData["ShortLink"] = shortenerLinkResponseAsObject?.ShortUrl;
                    }


                    TempData["LongLink"] = request.LonglinkModel.LongLink;

                    return RedirectToAction("Links");
                }

                TempData["ProcessLinkErrorMessage"] = "An unexpected error occurred.";

                return RedirectToAction("Links");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ShortenerAsync: " + ex.Message);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return View("Links");
            }
        }
        public async Task<IActionResult> RemoveLinkAsync(string code, CancellationToken ct = default)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];
                var RemoveLinkHttpResponseMessage =
                    await _shortenerServiceHttpRequestHandler.RemoveLink(authorizationCookie, code, ct);

                if (RemoveLinkHttpResponseMessage.IsSuccessStatusCode)
                {
                    var removeLinkResponseAsObject = await RemoveLinkHttpResponseMessage.Content.ReadFromJsonAsync<LinkRemoverResponseTransferObject>(new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true }, ct);


                    if (removeLinkResponseAsObject.Status == LinkRemoverResultStatus.Failed || removeLinkResponseAsObject.Status == LinkRemoverResultStatus.InvalidCode || removeLinkResponseAsObject.Status == LinkRemoverResultStatus.InvalidInformation)
                    {
                        TempData["RemoveLinkErrorMessage"] = removeLinkResponseAsObject.Message;
                    }

                    return View("Index");
                }

                return View("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //QRCode functions ----------------------------------------------
        public async Task<IActionResult> QrCode(CancellationToken ct = default)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                var receiveAllLinksHttpResponse =
                    await _shortenerServiceHttpRequestHandler.GetAllProcessedLinksByUserIdAsync(authorizationCookie, ct);
                if (receiveAllLinksHttpResponse.IsSuccessStatusCode)
                {
                    // Deserialize JSON content to list of ProcessedLinkViewModel
                    var receiveAllLinksResponseConvertedToObject = await receiveAllLinksHttpResponse.Content
                        .ReadFromJsonAsync<AllLinksFetcherResponseTransferObject>
                            (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);


                    if (receiveAllLinksResponseConvertedToObject.Status == AllLinksFetcherResultStatus.Failed)
                    {
                        TempData["FetchAllProcessLinksErrorMessage"] = receiveAllLinksResponseConvertedToObject.Message;
                    }

                    var result = new QRCodeModel()
                    {
                        LinksViewModel = receiveAllLinksResponseConvertedToObject
                    };

                    return View("QrCode", result);
                }
                //TempData["FetchAllProcessLinksErrorMessage"] = "An unexpected error occurred.";
                //return View("QrCode");
            }
            catch (JsonException ex)
            {
                // Log JSON parsing error
                Console.WriteLine($"Failed to parse JSON response: {ex.Message}");
                //return StatusCode(StatusCodes.Status502BadGateway, "Invalid response from the API.");
                TempData["FetchAllProcessLinksErrorMessage"] = "An unexpected error occurred.";

            }

            return View("QrCode");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateQrCode(QRCodeModel model, CancellationToken ct)
        {
            var authorizationCookie = Request?.Cookies["Authorization"];



            var linkShortenerServiceHttpResponse = await _shortenerServiceHttpRequestHandler
                .GetAllProcessedLinksByUserIdAsync(authorizationCookie, ct);


            if (!linkShortenerServiceHttpResponse.IsSuccessStatusCode)
            {
                TempData["QrError"] = "Failed to load your links.";
                return RedirectToAction("Index");
            }

            var linkShortenerServiceResponseAsObject = await linkShortenerServiceHttpResponse.Content
                .ReadFromJsonAsync<AllLinksFetcherResponseTransferObject>
                    (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

            var qrCodeServiceHttpResponse = await _qrCodeServiceHttpRequestHandler
                .GenerateQrCodeAsync(model.QrCodeGenerateRequest, authorizationCookie, ct);

            var result = new QRCodeModel
            {
                LinksViewModel = linkShortenerServiceResponseAsObject
            };

            if (!qrCodeServiceHttpResponse.IsSuccessStatusCode)
            {
                TempData["QrError"] = "QR Code generation failed.";
                return View("QrCode", result);
            }

            var qrDto = await qrCodeServiceHttpResponse.Content
                .ReadFromJsonAsync<QrCodeGenerateResponseTransferObject>
                    (new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);



            result.QrCode = qrDto;

            return View("QrCode", result);
        }



    }
}