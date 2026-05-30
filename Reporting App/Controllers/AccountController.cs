using Microsoft.AspNetCore.Mvc;
using Reporting_App.Models;
using System.Text;
using System.Text.Json;

namespace Reporting_App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("JwtToken") != null)
                return RedirectToAction("Index", "Reports");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("WebAPI");

            var payload = new { email = model.Email, password = model.Password };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password. Make sure you are a Training Coordinator.";
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            var token = result.GetProperty("token").GetString();
            var email = result.GetProperty("email").GetString();

            // Check role is Training Coordinator
            var roles = result.GetProperty("roles").EnumerateArray()
                .Select(r => r.GetString())
                .ToList();

            if (!roles.Contains("Training Coordinator"))
            {
                ViewBag.Error = "Access denied. This portal is for Training Coordinators only.";
                return View(model);
            }

            // Store token and user info in session
            HttpContext.Session.SetString("JwtToken", token!);
            HttpContext.Session.SetString("UserEmail", email!);

            return RedirectToAction("Index", "Reports");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}