using Core.Application.Dtos.Requests;
using Core.Domain.Entities;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IScheduleService
    {
        Task<Schedule> CreateScheduleAsync(CreateScheduleDto dto);
        Task<bool> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleDto dto);
        Task<bool> DeleteScheduleAsync(Guid scheduleId);
        Task<List<Schedule>> GetLecturerScheduleAsync(Guid lecturerId, int semester, string academicSession);
        Task<List<Schedule>> GetClassroomScheduleAsync(Guid classroomId, DayOfWeek? day = null);
    }
}
