namespace UniversityCourseManager.ViewModels
{
    public class DashboardViewModel
    {
        public bool IsAdmin { get; set; }
        public bool IsStudent { get; set; }

        public string UserEmail { get; set; } = string.Empty;

        public int SpecialtiesCount { get; set; }
        public int CoursesCount { get; set; }
        public int UsersCount { get; set; }
        public int StudentsCount { get; set; }
        public int EnrollmentsCount { get; set; }

        public int MyEnrolledCoursesCount { get; set; }
        public int MyTotalCredits { get; set; }

        public int RequiredCredits { get; set; }
        public int? ActiveSpecialtyId { get; set; }
        public string ActiveSpecialtyName { get; set; } = "Няма избрана специалност";
    }
}