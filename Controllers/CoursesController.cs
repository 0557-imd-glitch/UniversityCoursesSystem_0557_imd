using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManager.Data;
using UniversityCourseManager.Models;
using UniversityCourseManager.ViewModels;

namespace UniversityCourseManager.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.CourseSpecialties)
                    .ThenInclude(cs => cs.Specialty)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(courses);
        }

        // GET: Courses/Enrollments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enrollments()
        {
            var courses = await _context.Courses
                .Include(c => c.CourseSpecialties)
                    .ThenInclude(cs => cs.Specialty)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var courseIds = courses.Select(c => c.Id).ToList();

            var studentCourses = await _context.StudentCourses
                .Where(sc => courseIds.Contains(sc.CourseId))
                .ToListAsync();

            var studentIds = studentCourses
                .Select(sc => sc.StudentId)
                .Distinct()
                .ToList();

            var users = await _userManager.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.StudentEmails = users.ToDictionary(
                u => u.Id,
                u => u.Email ?? u.UserName ?? u.Id
            );

            ViewBag.StudentCourses = studentCourses;

            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, string? returnUrl = null)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                    .ThenInclude(cs => cs.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
                return NotFound();

            ViewBag.ReturnUrl = returnUrl;

            if (User.IsInRole("Student"))
            {
                var studentId = _userManager.GetUserId(User);

                if (!string.IsNullOrEmpty(studentId))
                {
                    ViewBag.IsEnrolled = await _context.StudentCourses
                        .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == course.Id);
                }
                else
                {
                    ViewBag.IsEnrolled = false;
                }
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = new CourseFormViewModel
            {
                AvailableSpecialties = await GetSpecialtiesSelectList()
            };

            return View(vm);
        }

        // POST: Courses/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableSpecialties = await GetSpecialtiesSelectList();
                return View(vm);
            }

            var course = new Course
            {
                Name = vm.Name,
                Lecturer = vm.Lecturer,
                Credits = vm.Credits,
                Annotation = vm.Annotation,
                CourseSpecialties = vm.SelectedSpecialtyIds
                    .Distinct()
                    .Select(id => new CourseSpecialty
                    {
                        SpecialtyId = id
                    })
                    .ToList()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            var vm = new CourseFormViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Lecturer = course.Lecturer,
                Credits = course.Credits,
                Annotation = course.Annotation,
                SelectedSpecialtyIds = course.CourseSpecialties
                    .Select(cs => cs.SpecialtyId)
                    .ToList(),
                AvailableSpecialties = await GetSpecialtiesSelectList()
            };

            return View(vm);
        }

        // POST: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                vm.AvailableSpecialties = await GetSpecialtiesSelectList();
                return View(vm);
            }

            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            course.Name = vm.Name;
            course.Lecturer = vm.Lecturer;
            course.Credits = vm.Credits;
            course.Annotation = vm.Annotation;

            course.CourseSpecialties.Clear();

            foreach (var specialtyId in vm.SelectedSpecialtyIds.Distinct())
            {
                course.CourseSpecialties.Add(new CourseSpecialty
                {
                    CourseId = course.Id,
                    SpecialtyId = specialtyId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                    .ThenInclude(cs => cs.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Enroll/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int id, string? returnUrl = null)
        {
            var studentId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized();

            var exists = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == id);

            if (exists)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction(nameof(Details), new { id });
            }

            var activeStudentSpecialty = await _context.StudentSpecialties
                .Include(ss => ss.Specialty)
                .FirstOrDefaultAsync(ss => ss.StudentId == studentId && ss.IsActive);

            if (activeStudentSpecialty == null)
            {
                TempData["EnrollError"] = "Моля, първо изберете активна специалност.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Specialties");
            }

            var course = await _context.Courses
                .Include(c => c.CourseSpecialties)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            var belongsToActiveSpecialty = course.CourseSpecialties
                .Any(cs => cs.SpecialtyId == activeStudentSpecialty.SpecialtyId);

            if (!belongsToActiveSpecialty)
            {
                TempData["EnrollError"] = "Тази дисциплина не принадлежи към активната специалност.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Courses", "Specialties", new { id = activeStudentSpecialty.SpecialtyId });
            }

            var earnedCredits = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Join(_context.Courses,
                    sc => sc.CourseId,
                    c => c.Id,
                    (sc, c) => c)
                .Where(c => c.CourseSpecialties.Any(cs => cs.SpecialtyId == activeStudentSpecialty.SpecialtyId))
                .SumAsync(c => (int?)c.Credits) ?? 0;

            var requiredCredits = activeStudentSpecialty.Specialty?.RequiredElectiveCredits ?? 0;
            var wouldExceed = earnedCredits + course.Credits > requiredCredits;

            if (wouldExceed)
            {
                TempData["EnrollError"] = "Вече сте достигнали или ще надвишите необходимия брой кредити. Отпишете друга дисциплина, преди да запишете нова.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Courses", "Specialties", new { id = activeStudentSpecialty.SpecialtyId });
            }

            _context.StudentCourses.Add(new StudentCourse
            {
                StudentId = studentId,
                CourseId = id
            });

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Courses/Unenroll/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Unenroll(int id, string? returnUrl = null)
        {
            var studentId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized();

            var row = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == id);

            if (row != null)
            {
                _context.StudentCourses.Remove(row);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<List<SelectListItem>> GetSpecialtiesSelectList()
        {
            return await _context.Specialties
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }
    }
}