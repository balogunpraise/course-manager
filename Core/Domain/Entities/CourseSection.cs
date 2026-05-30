using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class CourseSection : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public string SectionCode { get; set; } = string.Empty;  // e.g. CS101-A
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Schedule { get; set; } = string.Empty;     // e.g. "MWF 10:00-11:00"
        public string Location { get; set; } = string.Empty;
        public SectionStatus Status { get; set; } = SectionStatus.Open;

        public bool HasAvailableSlots => CurrentEnrollment < Capacity;

        // Navigation
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
        [ForeignKey(nameof(InstructorId))]
        public Instructor Instructor { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Waitlist> Waitlist { get; set; } = new List<Waitlist>();
    }
}
