
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.WebApp.attribute;
namespace ShrinkLink.WebApp.Controllers
{
    [JwtAuthorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
