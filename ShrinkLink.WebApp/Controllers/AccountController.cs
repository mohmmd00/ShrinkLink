using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;
using ShrinkLink.WebApp.Services;

namespace ShrinkLink.WebApp.Controllers
{
    public class AccountController : Controller
    {
        public readonly AuthServiceHttpRequestHandler _authServiceHttpRequestHandler;

        public AccountController(AuthServiceHttpRequestHandler authServiceHttpRequestHandler)
        {
            _authServiceHttpRequestHandler = authServiceHttpRequestHandler;
        }

        public IActionResult LoginPage()
        {

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View("LoginPage");
        }
        public IActionResult RegisterPage()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View("RegisterPage");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestTransferObject model)
        {
            var httpResponseMessage = await _authServiceHttpRequestHandler.PostRegisterRequest(model);
            try
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = httpResponseMessage.StatusCode switch
                    {
                        HttpStatusCode.BadRequest => $"Invalid Register request. please try again",
                        HttpStatusCode.Unauthorized => $"Invalid Username, EmailAddress or Password",
                        HttpStatusCode.ServiceUnavailable => $"Service Unavailable",
                        _ =>
                            $"Unable to login - Status: {(int)httpResponseMessage.StatusCode} - Error: {httpResponseMessage.ReasonPhrase}"
                    };

                    return RedirectToAction("RegisterPage");
                }
                var registerResponse =
                    await httpResponseMessage.Content.ReadFromJsonAsync<RegisterResponseTransferObject>(new JsonSerializerOptions
                        { PropertyNameCaseInsensitive = true });

                TempData["RegisterResponse"] = $"{registerResponse.Status} {registerResponse.Message}";
                
                return RedirectToAction("LoginPage");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestTransferObject model)
        {
            var httpResponseMessage = await _authServiceHttpRequestHandler.PostLoginRequest(model);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = httpResponseMessage.StatusCode switch
                {
                    HttpStatusCode.BadRequest => $"Invalid login request. please try again",
                    HttpStatusCode.Unauthorized => $"Invalid Username or password",
                    HttpStatusCode.ServiceUnavailable => $"Service Unavailable",
                    _ =>
                        $"Unable to login - Status: {(int)httpResponseMessage.StatusCode} - Error: {httpResponseMessage.ReasonPhrase}"
                };

                return RedirectToAction("LoginPage");
            }
            try
            {

                var loginResponse = await httpResponseMessage.Content.ReadFromJsonAsync<LoginResponseTransferObject>(new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true });

                Response.Cookies.Append("Authorization", loginResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = loginResponse.ExpiresAt,
                    Path = "/"
                });


                

                return Redirect($"/Dashboard/");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["operationMessage"] = "An unexpected error occurred during login. Please try again.";
                TempData["LoginModel"] = JsonSerializer.Serialize(model); // Preserve user input

                return RedirectToAction("LoginPage");
            }
        }
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                // Delete the cookie that holds the JWT
                Response.Cookies.Delete("Authorization", new CookieOptions
                {
                    Path = "/",
                    Secure = true,      // Set to true in production (HTTPS)
                    HttpOnly = true,    // Must match how it was set
                    SameSite = SameSiteMode.Strict
                });

                // Optional: Clear authentication context
                // Not strictly needed for stateless JWT, but clean
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

                return RedirectToAction("LoginPage", "Account");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logout error: " + ex.Message);
                TempData["ErrorMessage"] = "Unable to log out.";
                return RedirectToAction("LoginPage", "Account");
            }
        }
    }
}
