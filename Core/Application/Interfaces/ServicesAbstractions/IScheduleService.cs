using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IScheduleService
    {
        Task<BaseResponse<ScheduleResponseDto>> CreateScheduleAsync(CreateScheduleDto dto);
        Task<bool> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleDto dto);
        Task<bool> DeleteScheduleAsync(Guid scheduleId);
        Task<List<Schedule>> GetLecturerScheduleAsync(Guid lecturerId, Semester semester, Guid academicSessionId);
        Task<List<Schedule>> GetClassroomScheduleAsync(Guid classroomId, DayOfWeek? day = null);
    }
}
