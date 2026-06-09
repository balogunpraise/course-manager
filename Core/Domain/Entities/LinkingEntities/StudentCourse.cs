using Core.Domain.Enums;

namespace Core.Domain.Entities.LinkingEntities
{
    public class StudentCourse : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student{ get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime DroppedAt { get; set; }
        public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Enrolled;
        public Course Course { get; set; }
        public List<CourseActivity> CourseActivities { get; set; }
    }
}
