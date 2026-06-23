using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public Guid LecturerId { get; set; }
        [ForeignKey(nameof(LecturerId))]
        public Lecturer Lecturer { get; set; }

        public Guid ClassroomId { get; set; }
        [ForeignKey(nameof(ClassroomId))]
        public Classroom Classroom { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public ScheduleStatus Status { get; set; } 
        public Semester Semester { get; set; }
        public string AcademicSession { get; set; }  
        public Guid? AcademicSessionId { get; set; }
        [ForeignKey(nameof(AcademicSessionId))]
        public AcademicSession Session { get; set; }
    }
}
