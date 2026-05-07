using Microsoft.AspNetCore.Mvc;

namespace MVC_Frontend.Controllers
{
    public class AccountController : Controller
    {
        // Abdullah wires authentication logic into these actions
        public IActionResult Login()  => View();
        public IActionResult Register() => View();
    }
}
