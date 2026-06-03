using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Frontend.Models;
using System.Text.Json;

namespace MVC_Frontend.Controllers
{
    [AllowAnonymous]
    public class PublicController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PublicController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CertificationLookup()
        {
            return View(new CertificationLookupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CertificationLookup(CertificationLookupViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var url = $"api/public/certification?traineeId={model.TraineeId}&certificateNumber={Uri.EscapeDataString(model.CertificateNumber)}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model.Result = JsonSerializer.Deserialize<CertificationLookupResult>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    model.ErrorMessage = "No certificate found matching the provided details. Please check the Trainee ID and Certificate Number.";
                }
                else
                {
                    model.ErrorMessage = "An error occurred while verifying the certificate. Please try again.";
                }
            }
            catch
            {
                model.ErrorMessage = "Could not reach the verification service. Please try again later.";
            }

            return View(model);
        }
    }
}
