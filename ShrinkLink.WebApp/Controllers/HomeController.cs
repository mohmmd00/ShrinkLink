using Microsoft.AspNetCore.Mvc;
using ShrinkLink.WebApp.Models;
using System.Diagnostics;

namespace ShrinkLink.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RedirectBasedOnAuth()
        {
            var authorizationCookie = Request.Cookies["Authorization"];
            if (!string.IsNullOrEmpty(authorizationCookie))
            {
                return Redirect($"/Dashboard/Index");
            }
            else
            {
                return Redirect($"/Account/LoginPage");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
