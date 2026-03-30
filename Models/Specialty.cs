using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManager.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        [Display(Name = "Име")]
        [Required(ErrorMessage = "Името на специалността е задължително.")]
        [StringLength(100, ErrorMessage = "Името не може да е по-дълго от 100 символа.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Необходими избираеми кредити")]
        [Range(0, 300, ErrorMessage = "Стойността трябва да е между 0 и 300.")]
        public int RequiredElectiveCredits { get; set; }

        public ICollection<CourseSpecialty> CourseSpecialties { get; set; } = new List<CourseSpecialty>();
    }
}