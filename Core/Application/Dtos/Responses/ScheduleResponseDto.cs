using Core.Domain.Enums;

namespace Core.Application.Dtos.Responses
{
    public class ScheduleResponseDto
    {
        public Guid CourseId { get; set; }
        public string Course { get; set; }
        public Guid LecturerId { get; set; }
        public string Lecturer { get; set; }
        public Guid ClassroomId { get; set; }
        public string ClassRoom { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public ScheduleStatus Status { get; set; }
        public string AcademicSession { get; set; }
        public int Semester { get; set; }
    }
}
