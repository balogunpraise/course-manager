using Core.Domain.Enums;

namespace Core.Application.Dtos.Requests
{
    public class GetScheduleRequest
    {
        public Guid LecturerId { get; set; }
        public Semester Semester { get; set; }
        public Guid AcademicSessionId { get; set; }
    }

    public class GetMyScheduleRequest
    {
        public Semester Semester { get; set; }
        public Guid AcademicSessionId { get; set; }
    }
}
