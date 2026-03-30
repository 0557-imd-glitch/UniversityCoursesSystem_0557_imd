using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManager.Models;

namespace UniversityCourseManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseSpecialty> CourseSpecialties => Set<CourseSpecialty>();
        public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
        public DbSet<StudentSpecialty> StudentSpecialties => Set<StudentSpecialty>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CourseSpecialty>()
                .HasKey(cs => new { cs.CourseId, cs.SpecialtyId });

            builder.Entity<CourseSpecialty>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSpecialties)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseSpecialty>()
                .HasOne(cs => cs.Specialty)
                .WithMany(s => s.CourseSpecialties)
                .HasForeignKey(cs => cs.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany()
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentCourse>()
                .HasIndex(sc => new { sc.StudentId, sc.CourseId })
                .IsUnique();

            builder.Entity<StudentSpecialty>()
                .HasOne(ss => ss.Specialty)
                .WithMany()
                .HasForeignKey(ss => ss.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentSpecialty>()
                .HasIndex(ss => new { ss.StudentId, ss.SpecialtyId })
                .IsUnique();
        }
    }
}