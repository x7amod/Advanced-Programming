using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TrainingInstituteDBContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TrainingInstituteDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ?? GET: /Account/Login ??????????????????????????????????????
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // ?? POST: /Account/Login ?????????????????????????????????????
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ViewBag.Error = "Account locked. Too many failed attempts. Please try again in 5 minutes.";
                return View(model);
            }

            ViewBag.Error = "Invalid email or password. Please try again.";
            return View(model);
        }

        // ?? GET: /Account/Register ???????????????????????????????????
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        // ?? POST: /Account/Register ??????????????????????????????????
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!model.AgreeTerms)
            {
                ViewBag.Error = "You must agree to the terms of service.";
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            // Create the Identity user
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign Trainee role
                await _userManager.AddToRoleAsync(user, "Trainee");

                // Create Trainee profile record
                var trainee = new Trainee
                {
                    UserId = user.Id,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    EmergencyContact = model.EmergencyContact
                };

                _context.Trainees.Add(trainee);
                await _context.SaveChangesAsync();

                // Sign in immediately after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Success"] = "Welcome to Taalam! Your account has been created.";
                return RedirectToAction("Index", "Home");
            }

            // Show Identity errors
            ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
            return View(model);
        }

        // ?? POST: /Account/Logout ????????????????????????????????????
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // ?? GET: /Account/AccessDenied ???????????????????????????????
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}