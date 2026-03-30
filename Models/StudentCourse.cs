using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManager.Models
{
    public class StudentCourse
    {
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public Course? Course { get; set; }
    }
}