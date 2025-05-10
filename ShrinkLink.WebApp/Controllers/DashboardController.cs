using Microsoft.AspNetCore.Mvc;

namespace ShrinkLink.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
