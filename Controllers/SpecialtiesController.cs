using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManager.Data;
using UniversityCourseManager.Models;

namespace UniversityCourseManager.Controllers
{
    [Authorize]
    public class SpecialtiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialtiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            var specialties = await _context.Specialties
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(specialties);
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialty = await _context.Specialties
                .Include(s => s.CourseSpecialties)
                    .ThenInclude(cs => cs.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (specialty == null)
                return NotFound();

            return View(specialty);
        }

        // GET: Specialties/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialty specialty)
        {
            if (!ModelState.IsValid)
                return View(specialty);

            _context.Add(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
                return NotFound();

            return View(specialty);
        }

        // POST: Specialties/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Specialty specialty)
        {
            if (id != specialty.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(specialty);

            try
            {
                _context.Update(specialty);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialtyExists(specialty.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);

            if (specialty == null)
                return NotFound();

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
                _context.Specialties.Remove(specialty);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Courses/5
        public async Task<IActionResult> Courses(int id)
        {
            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialty == null)
                return NotFound();

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(studentId) && User.IsInRole("Student"))
            {
                var existingSpecialties = await _context.StudentSpecialties
                    .Where(ss => ss.StudentId == studentId)
                    .ToListAsync();

                foreach (var ss in existingSpecialties)
                {
                    ss.IsActive = ss.SpecialtyId == id;
                }

                if (!existingSpecialties.Any(ss => ss.SpecialtyId == id))
                {
                    _context.StudentSpecialties.Add(new StudentSpecialty
                    {
                        StudentId = studentId,
                        SpecialtyId = id,
                        IsActive = true
                    });
                }

                await _context.SaveChangesAsync();
            }

            var courses = await _context.Courses
                .Include(c => c.CourseSpecialties)
                    .ThenInclude(cs => cs.Specialty)
                .Where(c => c.CourseSpecialties.Any(cs => cs.SpecialtyId == id))
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.SpecialtyName = specialty.Name;
            ViewBag.SpecialtyId = specialty.Id;
            ViewBag.RequiredElectiveCredits = specialty.RequiredElectiveCredits;

            if (!string.IsNullOrEmpty(studentId) && User.IsInRole("Student"))
            {
                var enrolledCourseIds = await _context.StudentCourses
                    .Where(sc => sc.StudentId == studentId)
                    .Select(sc => sc.CourseId)
                    .ToListAsync();

                ViewBag.EnrolledIds = enrolledCourseIds;

                var earnedCredits = await _context.StudentCourses
                    .Where(sc => sc.StudentId == studentId)
                    .Join(_context.Courses,
                        sc => sc.CourseId,
                        c => c.Id,
                        (sc, c) => c)
                    .Where(c => c.CourseSpecialties.Any(cs => cs.SpecialtyId == id))
                    .SumAsync(c => (int?)c.Credits) ?? 0;

                var remainingCredits = specialty.RequiredElectiveCredits - earnedCredits;
                if (remainingCredits < 0)
                    remainingCredits = 0;

                ViewBag.EarnedCredits = earnedCredits;
                ViewBag.RemainingCredits = remainingCredits;
                ViewBag.HasEnoughCredits = earnedCredits >= specialty.RequiredElectiveCredits;
            }

            return View(courses);
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}