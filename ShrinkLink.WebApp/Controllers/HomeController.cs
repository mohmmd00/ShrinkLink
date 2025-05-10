using Microsoft.AspNetCore.Mvc;
using ShrinkLink.WebApp.Models;
using System.Diagnostics;
using ShrinkLink.WebApp.Models.Dtos;
using ShrinkLink.WebApp.Services;

namespace ShrinkLink.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly WebRequestHandlerApplication _application;

        public HomeController(ILogger<HomeController> logger, WebRequestHandlerApplication application)
        {
            _logger = logger;
            _application = application;
        }


        [HttpPost]
        public async Task<IActionResult> PostLongUrl(ShortenerViewModel model)
        {

            Console.WriteLine(model.Request.LongLink);
            var result = await _application.PostLinkHandlerAsync(model.Request);
            Console.WriteLine(result.ShortUrl);


            var test = new ShortenerViewModel
            {
                Request = model.Request,
                Response = result
            };


            return View("Index",test);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
