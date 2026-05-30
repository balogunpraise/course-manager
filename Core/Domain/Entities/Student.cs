using Core.Domain.Entities.Auth;
using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
        public decimal GPA { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid LevelId { get; set; }

        // Navigation
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Transcript> Transcripts { get; set; } = new List<Transcript>();
        public ICollection<Waitlist> WaitlistEntries { get; set; } = new List<Waitlist>();
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; }
    }

}
