using Core.Domain.Enums;


namespace Core.Application.Dtos.Requests
{
    public class CreateScheduleDto
    {
        public Guid CourseId { get; set; }
        public Guid LecturerId { get; set; }
        public Guid ClassroomId { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Semester { get; set; }
        public string AcademicSession { get; set; }
    }

    public class UpdateScheduleDto : CreateScheduleDto
    {
        public ScheduleStatus Status { get; set; }
    }
}
