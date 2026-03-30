using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManager.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Display(Name = "Име")]
        [Required(ErrorMessage = "Името на дисциплината е задължително.")]
        [StringLength(100, ErrorMessage = "Името не може да е по-дълго от 100 символа.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Преподавател")]
        [StringLength(100, ErrorMessage = "Името на преподавателя не може да е по-дълго от 100 символа.")]
        public string? Lecturer { get; set; }

        [Display(Name = "Кредити")]
        [Range(0, 60, ErrorMessage = "Кредитите трябва да са между 0 и 60.")]
        public int Credits { get; set; }

        [Display(Name = "Анотация")]
        public string? Annotation { get; set; }

        public ICollection<CourseSpecialty> CourseSpecialties { get; set; } = new List<CourseSpecialty>();

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}