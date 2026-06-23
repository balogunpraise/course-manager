using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ScheduleConflictChecker _conflictChecker;

        public ScheduleService(ApplicationDbContext context, ScheduleConflictChecker conflictChecker)
        {
            _context = context;
            _conflictChecker = conflictChecker;
        }

        public async Task<BaseResponse<ScheduleResponseDto>> CreateScheduleAsync(CreateScheduleDto dto)
        {
            // Validate course exists
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return BaseResponse<ScheduleResponseDto>.Failure(404, "Course not found");

            // Validate lecturer exists
            var lecturer = await _context.Lecturers.FindAsync(dto.LecturerId);
            if (lecturer == null)
                return BaseResponse<ScheduleResponseDto>.Failure(404, "Lecturer not found");

            // Validate classroom exists
            var classroom = await _context.Classrooms.FindAsync(dto.ClassroomId);
            if (classroom == null)
                return BaseResponse<ScheduleResponseDto>.Failure(404, "Classroom not found");

            // Check lecturer course load
            if (!_conflictChecker.LecturerHasCourseLoadCapacity(dto.LecturerId, dto.Semester, dto.AcademicSession))
                return BaseResponse<ScheduleResponseDto>.Failure(400, "Lecturer has exceeded maximum course load for this session");

            var schedule = new Schedule
            {
                CourseId = dto.CourseId,
                LecturerId = dto.LecturerId,
                ClassroomId = dto.ClassroomId,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Semester = dto.Semester,
                AcademicSession = dto.AcademicSession,
                AcademicSessionId = dto.AcademicSessionId,
                Status = ScheduleStatus.Approved
            };

            // Check for conflicts
            var conflicts = _conflictChecker.CheckAllConflicts(schedule);
            if (!conflicts.IsValid)
                return BaseResponse<ScheduleResponseDto>.Failure(400, $"Schedule conflicts: {string.Join(", ", conflicts.Conflicts)}");

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            var scheduleResponse = new ScheduleResponseDto
            {
                CourseId = dto.CourseId,
                Course = course.CourseCode,
                LecturerId = dto.LecturerId,
                Lecturer = $"{lecturer.FirstName} {lecturer.LastName}",
                ClassroomId = dto.ClassroomId,
                ClassRoom = classroom.Name,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = ScheduleStatus.Approved,
                AcademicSession = dto.AcademicSession,
                Semester = dto.Semester
            };

            return BaseResponse<ScheduleResponseDto>.Success(200, "success", scheduleResponse);
        }

        public Task<bool> DeleteScheduleAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Schedule>> GetClassroomScheduleAsync(Guid classroomId, DayOfWeek? day = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Schedule>> GetLecturerScheduleAsync(Guid lecturerId, Semester semester, Guid academicSessionId)
        {
            return await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .Where(s => s.LecturerId == lecturerId &&
                            s.Semester == semester &&
                            s.AcademicSessionId == academicSessionId &&
                            !s.IsDeleted)
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public Task<bool> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleDto dto)
        {
            throw new NotImplementedException();
        }

        // Implement other methods...
    }
}
