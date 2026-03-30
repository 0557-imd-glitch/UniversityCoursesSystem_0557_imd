using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManager.Models
{
    public class StudentSpecialty
    {
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public int SpecialtyId { get; set; }

        public Specialty? Specialty { get; set; }

        public bool IsActive { get; set; }
    }
}