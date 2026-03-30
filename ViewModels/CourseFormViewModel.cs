using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UniversityCourseManager.ViewModels
{
    public class CourseFormViewModel
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

        [Display(Name = "Специалности")]
        public List<int> SelectedSpecialtyIds { get; set; } = new();

        public List<SelectListItem> AvailableSpecialties { get; set; } = new();
    }
}