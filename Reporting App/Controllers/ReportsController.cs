using Microsoft.AspNetCore.Mvc;
using Reporting_App.Models;
using System.Text.Json;

namespace Reporting_App.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient("WebAPI");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private IActionResult RedirectIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("Login", "Account");
            return null!;
        }

        // GET /Reports/Index
        public async Task<IActionResult> Index()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var client = GetAuthorizedClient();
            var response = await client.GetAsync("/api/reports/overview");

            if (!response.IsSuccessStatusCode)
                return View(new OverviewViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var model = JsonSerializer.Deserialize<OverviewViewModel>(json, options)
                ?? new OverviewViewModel();

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View(model);
        }

        // GET /Reports/Enrollments
        public async Task<IActionResult> Enrollments()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var client = GetAuthorizedClient();
            var response = await client.GetAsync("/api/reports/enrollments");

            if (!response.IsSuccessStatusCode)
                return View(new EnrollmentStatsViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var model = JsonSerializer.Deserialize<EnrollmentStatsViewModel>(json, options)
                ?? new EnrollmentStatsViewModel();

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View(model);
        }

        // GET /Reports/Instructors
        public async Task<IActionResult> Instructors()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var client = GetAuthorizedClient();
            var response = await client.GetAsync("/api/reports/instructors");

            if (!response.IsSuccessStatusCode)
                return View(new InstructorStatsViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var model = JsonSerializer.Deserialize<InstructorStatsViewModel>(json, options)
                ?? new InstructorStatsViewModel();

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View(model);
        }

        // GET /Reports/Certifications
        public async Task<IActionResult> Certifications()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var client = GetAuthorizedClient();
            var response = await client.GetAsync("/api/reports/certifications");

            if (!response.IsSuccessStatusCode)
                return View(new CertificationStatsViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var model = JsonSerializer.Deserialize<CertificationStatsViewModel>(json, options)
                ?? new CertificationStatsViewModel();

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View(model);
        }

        // GET /Reports/Revenue
        public async Task<IActionResult> Revenue()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var client = GetAuthorizedClient();
            var response = await client.GetAsync("/api/reports/revenue");

            if (!response.IsSuccessStatusCode)
                return View(new RevenueStatsViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var model = JsonSerializer.Deserialize<RevenueStatsViewModel>(json, options)
                ?? new RevenueStatsViewModel();

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View(model);
        }
    }
}