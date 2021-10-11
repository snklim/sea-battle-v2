using Microsoft.AspNetCore.Mvc;

namespace SeaBattle.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}