using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManager.Data;
using UniversityCourseManager.Models;
using UniversityCourseManager.ViewModels;

namespace UniversityCourseManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var usersCount = await _userManager.Users.CountAsync();

            var model = new DashboardViewModel
            {
                IsAdmin = User.IsInRole("Admin"),
                IsStudent = User.IsInRole("Student"),
                UserEmail = User.Identity?.Name ?? string.Empty,

                SpecialtiesCount = await _context.Specialties.CountAsync(),
                CoursesCount = await _context.Courses.CountAsync(),

                UsersCount = usersCount,
                StudentsCount = usersCount,

                EnrollmentsCount = await _context.StudentCourses.CountAsync()
            };

            if (!string.IsNullOrEmpty(userId) && model.IsStudent)
            {
                model.MyEnrolledCoursesCount = await _context.StudentCourses
                    .Where(sc => sc.StudentId == userId)
                    .CountAsync();

                var activeStudentSpecialty = await _context.StudentSpecialties
                    .Include(ss => ss.Specialty)
                    .Where(ss => ss.StudentId == userId && ss.IsActive)
                    .FirstOrDefaultAsync();

                if (activeStudentSpecialty == null)
                {
                    return RedirectToAction("Index", "Specialties");
                }

                ViewBag.RequiredCredits = activeStudentSpecialty.Specialty?.RequiredElectiveCredits ?? 1;
                ViewBag.ActiveSpecialtyName = activeStudentSpecialty.Specialty?.Name ?? "";
                ViewBag.ActiveSpecialtyId = activeStudentSpecialty.SpecialtyId;

                model.MyTotalCredits = await _context.StudentCourses
                    .Where(sc => sc.StudentId == userId)
                    .Join(_context.Courses,
                        sc => sc.CourseId,
                        c => c.Id,
                        (sc, c) => c)
                    .Where(c => c.CourseSpecialties.Any(cs => cs.SpecialtyId == activeStudentSpecialty.SpecialtyId))
                    .SumAsync(c => (int?)c.Credits) ?? 0;

                var enrolledCourses = await _context.StudentCourses
                    .Where(sc => sc.StudentId == userId)
                    .Include(sc => sc.Course)
                        .ThenInclude(c => c!.CourseSpecialties)
                            .ThenInclude(cs => cs.Specialty)
                    .Where(sc => sc.Course!.CourseSpecialties.Any(cs => cs.SpecialtyId == activeStudentSpecialty.SpecialtyId))
                    .Select(sc => new
                    {
                        Name = sc.Course!.Name,
                        Credits = sc.Course.Credits,
                        SpecialtyName = activeStudentSpecialty.Specialty!.Name
                    })
                    .ToListAsync();

                ViewBag.EnrolledCourses = enrolledCourses;
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}