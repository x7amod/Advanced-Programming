using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Frontend.Controllers
{

    [AllowAnonymous]
    public class LandingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {

            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}