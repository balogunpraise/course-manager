namespace Core.Domain.Entities
{
    public class Instructor : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? OfficeLocation { get; set; }

        // Navigation
        public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
    }
}
