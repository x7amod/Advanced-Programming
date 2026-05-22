using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize(Roles = AppRoles.Coordinator)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TrainingInstituteDBContext _context;

        public UserManagementController(UserManager<IdentityUser> userManager, TrainingInstituteDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var instructorData = await _context.Instructors.ToListAsync();
            var traineeData = await _context.Trainees.ToListAsync();

            var allUserIds = instructorData.Select(i => i.UserId)
                .Concat(traineeData.Select(t => t.UserId))
                .Distinct()
                .ToList();

            var allIdentityUsers = await _context.Users.ToListAsync();
            var identityUserMap = allIdentityUsers
                .Where(u => allUserIds.Contains(u.Id))
                .ToDictionary(u => u.Id);

            var users = new List<UserListViewModel>();

            foreach (var instructor in instructorData)
            {
                if (!identityUserMap.TryGetValue(instructor.UserId, out var user)) continue;
                users.Add(new UserListViewModel
                {
                    UserId = user.Id,
                    Username = user.Email ?? "",
                    Email = user.Email ?? "",
                    Phone = user.PhoneNumber ?? "",
                    Role = AppRoles.Instructor,
                    ProfileId = instructor.InstructorId
                });
            }

            foreach (var trainee in traineeData)
            {
                if (!identityUserMap.TryGetValue(trainee.UserId, out var user)) continue;
                users.Add(new UserListViewModel
                {
                    UserId = user.Id,
                    Username = user.Email ?? "",
                    Email = user.Email ?? "",
                    Phone = user.PhoneNumber ?? "",
                    Role = AppRoles.Trainee,
                    ProfileId = trainee.TraineeId
                });
            }

            return View(users.OrderBy(u => u.Role).ThenBy(u => u.Username).ToList());
        }

        // ─── Instructor ─────────────────────────────────────────────

        // GET: UserManagement/CreateInstructor
        public IActionResult CreateInstructor() => View(new CreateInstructorViewModel());

        // POST: UserManagement/CreateInstructor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstructor(CreateInstructorViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _userManager.FindByEmailAsync(vm.Email) != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists.");
                return View(vm);
            }

            var user = new IdentityUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, AppRoles.Instructor);

            _context.Instructors.Add(new Instructor
            {
                UserId = user.Id,
                HireDate = vm.HireDate,
                Bio = vm.Bio?.Trim()
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Instructor {vm.Email} created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/EditInstructor/5
        public async Task<IActionResult> EditInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            var user = await _userManager.FindByIdAsync(instructor.UserId);
            if (user == null) return NotFound();

            return View(new EditInstructorViewModel
            {
                UserId = user.Id,
                InstructorId = instructor.InstructorId,
                Username = user.Email ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                HireDate = instructor.HireDate,
                Bio = instructor.Bio
            });
        }

        // POST: UserManagement/EditInstructor/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstructor(int id, EditInstructorViewModel vm)
        {
            if (id != vm.InstructorId) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            var existingWithEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existingWithEmail != null && existingWithEmail.Id != user.Id)
            {
                ModelState.AddModelError(nameof(vm.Email), "This email is already in use by another account.");
                return View(vm);
            }

            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.PhoneNumber = vm.Phone;
            await _userManager.UpdateAsync(user);

            instructor.HireDate = vm.HireDate;
            instructor.Bio = vm.Bio?.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Instructor {vm.Email} updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/DeleteInstructor/5
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.CourseSessions)
                .Include(i => i.Assessments)
                .FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null) return NotFound();

            var user = await _userManager.FindByIdAsync(instructor.UserId);
            if (user == null) return NotFound();

            ViewBag.FullName = user.Email ?? "—";
            ViewBag.Email = user.Email;
            ViewBag.HasSessions = instructor.CourseSessions?.Any() ?? false;
            ViewBag.HasAssessments = instructor.Assessments?.Any() ?? false;
            return View(instructor);
        }

        // POST: UserManagement/DeleteInstructor/5
        [HttpPost, ActionName("DeleteInstructor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructorConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.CourseSessions)
                .Include(i => i.Assessments)
                .FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null) return NotFound();

            if (instructor.CourseSessions?.Any() == true)
            {
                TempData["Error"] = "Cannot delete an instructor who is assigned to course sessions.";
                return RedirectToAction(nameof(Index));
            }
            if (instructor.Assessments?.Any() == true)
            {
                TempData["Error"] = "Cannot delete an instructor who has assessments on record.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(instructor.UserId);
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            if (user != null) await _userManager.DeleteAsync(user);

            TempData["Success"] = "Instructor account deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Trainee ─────────────────────────────────────────────────

        // GET: UserManagement/CreateTrainee
        public IActionResult CreateTrainee() => View(new CreateTraineeViewModel());

        // POST: UserManagement/CreateTrainee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainee(CreateTraineeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _userManager.FindByEmailAsync(vm.Email) != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists.");
                return View(vm);
            }

            var user = new IdentityUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, AppRoles.Trainee);

            _context.Trainees.Add(new Trainee
            {
                UserId = user.Id,
                DateOfBirth = vm.DateOfBirth,
                Address = vm.Address.Trim(),
                EmergencyContact = vm.EmergencyContact.Trim()
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Trainee {vm.Email} created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/EditTrainee/5
        public async Task<IActionResult> EditTrainee(int id)
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return NotFound();

            var user = await _userManager.FindByIdAsync(trainee.UserId);
            if (user == null) return NotFound();

            return View(new EditTraineeViewModel
            {
                UserId = user.Id,
                TraineeId = trainee.TraineeId,
                Username = user.Email ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                DateOfBirth = trainee.DateOfBirth,
                Address = trainee.Address,
                EmergencyContact = trainee.EmergencyContact
            });
        }

        // POST: UserManagement/EditTrainee/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainee(int id, EditTraineeViewModel vm)
        {
            if (id != vm.TraineeId) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return NotFound();

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            var existingWithEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existingWithEmail != null && existingWithEmail.Id != user.Id)
            {
                ModelState.AddModelError(nameof(vm.Email), "This email is already in use by another account.");
                return View(vm);
            }

            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.PhoneNumber = vm.Phone;
            await _userManager.UpdateAsync(user);

            trainee.DateOfBirth = vm.DateOfBirth;
            trainee.Address = vm.Address.Trim();
            trainee.EmergencyContact = vm.EmergencyContact.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Trainee {vm.Email} updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/DeleteTrainee/5
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            var trainee = await _context.Trainees
                .Include(t => t.Enrollments)
                .FirstOrDefaultAsync(t => t.TraineeId == id);
            if (trainee == null) return NotFound();

            var user = await _userManager.FindByIdAsync(trainee.UserId);
            if (user == null) return NotFound();

            ViewBag.FullName = user.Email ?? "—";
            ViewBag.Email = user.Email;
            ViewBag.HasEnrollments = trainee.Enrollments?.Any() ?? false;
            return View(trainee);
        }

        // POST: UserManagement/DeleteTrainee/5
        [HttpPost, ActionName("DeleteTrainee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTraineeConfirmed(int id)
        {
            var trainee = await _context.Trainees
                .Include(t => t.Enrollments)
                .FirstOrDefaultAsync(t => t.TraineeId == id);
            if (trainee == null) return NotFound();

            if (trainee.Enrollments?.Any() == true)
            {
                TempData["Error"] = "Cannot delete a trainee who has enrollment records.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(trainee.UserId);
            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            if (user != null) await _userManager.DeleteAsync(user);

            TempData["Success"] = "Trainee account deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Change Role ─────────────────────────────────────────────

        // GET: UserManagement/ChangeRole?userId=...
        public async Task<IActionResult> ChangeRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

            if (currentRole != AppRoles.Trainee && currentRole != AppRoles.Instructor)
            {
                TempData["Error"] = "Role changes are only supported between Trainee and Instructor.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ChangeRoleViewModel
            {
                UserId = user.Id,
                Username = user.Email ?? "—",
                Email = user.Email ?? "",
                CurrentRole = currentRole ?? ""
            });
        }

        // POST: UserManagement/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

            if (currentRole == vm.NewRole)
            {
                ModelState.AddModelError(nameof(vm.NewRole), "The user already has this role.");
                return View(vm);
            }

            if (vm.NewRole != AppRoles.Trainee && vm.NewRole != AppRoles.Instructor)
            {
                ModelState.AddModelError(nameof(vm.NewRole), "Invalid role selection.");
                return View(vm);
            }

            if (currentRole != null)
                await _userManager.RemoveFromRoleAsync(user, currentRole);
            await _userManager.AddToRoleAsync(user, vm.NewRole);

            if (currentRole == AppRoles.Instructor && vm.NewRole == AppRoles.Trainee)
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);
                if (instructor != null)
                {
                    _context.Instructors.Remove(instructor);
                    await _context.SaveChangesAsync();
                }

                var trainee = new Trainee
                {
                    UserId = user.Id,
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Address = "Not set",
                    EmergencyContact = "Not set"
                };
                _context.Trainees.Add(trainee);
                await _context.SaveChangesAsync();

                TempData["Info"] = "Role changed to Trainee. Please complete the trainee's details.";
                return RedirectToAction(nameof(EditTrainee), new { id = trainee.TraineeId });
            }
            else
            {
                var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (trainee != null)
                {
                    _context.Trainees.Remove(trainee);
                    await _context.SaveChangesAsync();
                }

                var instructor = new Instructor
                {
                    UserId = user.Id,
                    HireDate = DateTime.Today
                };
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();

                TempData["Info"] = "Role changed to Instructor. Please complete the instructor's details.";
                return RedirectToAction(nameof(EditInstructor), new { id = instructor.InstructorId });
            }
        }
    }
}
