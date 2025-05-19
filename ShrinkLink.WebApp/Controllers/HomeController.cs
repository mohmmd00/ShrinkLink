using Microsoft.AspNetCore.Mvc;
using ShrinkLink.WebApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using ShrinkLink.WebApp.Models.Dtos;
using ShrinkLink.WebApp.Services;
using NuGet.Common;
using ShrinkLink.WebApp.attribute;

namespace ShrinkLink.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly WebRequestHandler _application;

        public HomeController(ILogger<HomeController> logger, WebRequestHandler application)
        {
            _logger = logger;
            _application = application;
        }
        public IActionResult Index()
        {
            return View();
        }

        [JwtAuthorize]
        public IActionResult RedirectToDashboard()
        {
            return Redirect("/Dashboard/");
        }

        public IActionResult RedirectBaseedOnAuth()
        {
            var fetchedToken = Request.Cookies["jwt-token"];
            if (!string.IsNullOrEmpty(fetchedToken))
            {
                return RedirectToAction("RedirectToDashboard");
            }
            else
            {
                return RedirectToAction("LoginPage");
            }
        }

        public IActionResult RegisterPage()
        {
            return View("RegisterPage");
        }

        public IActionResult LoginPage()
        {
            return View("LoginPage");
        }


        //aslan in inja nabayad bashe in bayad too dashboard bashe 
        //[HttpPost]
        //public async Task<IActionResult> PostLongUrl(ShortenerModel request)
        //{
        //    var token = Request.Cookies["jwt-token"];

        //    if (token == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var result = await _application.PostLinkHandlerAsync(request.Request, token);
        //    var test = new ShortenerModel
        //    {
        //        Request = request.Request,
        //        Response = result
        //    };

        //    return View("Index", test);
        //}



        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterModel model)
        //{
        //    var result = _application.PostRegisterRequest(model.TransferObject);


        //}






        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _application.PostLoginRequest(model.TransferObject);
            Response.Cookies.Append("jwt-token", result.Token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            });

            if (result.Token != null)
            {
                return RedirectToAction("RedirectToDashboard");
            }

            return BadRequest("can not login"); // this must be worked at
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
