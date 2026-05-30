using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CourseSectionId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? DroppedAt { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

        // Navigation
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;
        [ForeignKey(nameof(CourseSectionId))]
        public CourseSection CourseSection { get; set; } = null!;
        public Guid? GradeId { get; set; }
        [ForeignKey(nameof(GradeId))]
        public Grade Grade { get; set; } = null!;
    }
}
