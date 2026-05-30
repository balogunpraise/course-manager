using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Prerequisite : BaseEntity
    {
        public Guid? CourseId { get; set; }
        public Guid? RequiredCourseId { get; set; }
        public string MinimumGrade { get; set; }   // optional: must pass with at least "C"

        // Navigation
        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;
        public Course RequiredCourse { get; set; } = null!;
    }
}
