namespace UniversityCourseManager.Models
{
    public class CourseSpecialty
    {
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
    }
}