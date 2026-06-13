using AutoMapper;
using Core.Application.Dtos;
using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Infrastructure.data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduleController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ScheduleConflictChecker _conflictChecker;
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(
            ApplicationDbContext context,
            ScheduleConflictChecker conflictChecker,
            IScheduleService scheduleService,
            IMapper mapper)
        {
            _context = context;
            _conflictChecker = conflictChecker;
            _scheduleService = scheduleService;
            _mapper = mapper;
        }

        // GET: api/schedule
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllSchedules(
            [FromQuery] int? semester = null,
            [FromQuery] string academicSession = null,
            [FromQuery] DayOfWeek? day = null)
        {
            var query = _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Include(s => s.Classroom)
                .Where(s => !s.IsDeleted);

            if (semester.HasValue)
                query = query.Where(s => s.Semester == semester.Value);

            if (!string.IsNullOrEmpty(academicSession))
                query = query.Where(s => s.AcademicSession == academicSession);

            if (day.HasValue)
                query = query.Where(s => s.Day == day.Value);

            var schedules = await query
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ScheduleDto>>(schedules)));
        }

        // GET: api/schedule/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<ScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ScheduleDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSchedule(Guid id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (schedule == null)
                return Ok(BaseResponse<ScheduleDto>.Failure(StatusCodes.Status404NotFound, $"Schedule with ID {id} not found"));

            return Ok(BaseResponse<ScheduleDto>.Success(StatusCodes.Status200OK, "success", _mapper.Map<ScheduleDto>(schedule)));
        }

        // GET: api/schedule/lecturer/{lecturerId}
        [HttpGet("lecturer/{lecturerId}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetLecturerSchedules(
            Guid lecturerId,
            [FromQuery] int semester,
            [FromQuery] string academicSession)
        {
            var schedules = await _scheduleService.GetLecturerScheduleAsync(lecturerId, semester, academicSession);
            return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ScheduleDto>>(schedules)));
        }

        // GET: api/schedule/classroom/{classroomId}
        [HttpGet("classroom/{classroomId}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetClassroomSchedules(
            Guid classroomId,
            [FromQuery] DayOfWeek? day = null)
        {
            var query = _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Where(s => s.ClassroomId == classroomId && !s.IsDeleted);  

            if (day.HasValue)
                query = query.Where(s => s.Day == day.Value);

            var schedules = await query
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ScheduleDto>>(schedules)));
        }

        // GET: api/schedule/course/{courseId}
        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetCourseSchedules(
            Guid courseId,
            [FromQuery] string academicSession = null)
        {
            var query = _context.Schedules
                .Include(s => s.Lecturer)
                .Include(s => s.Classroom)
                .Where(s => s.CourseId == courseId && !s.IsDeleted);

            if (!string.IsNullOrEmpty(academicSession))
                query = query.Where(s => s.AcademicSession == academicSession);

            var schedules = await query.ToListAsync();
            return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ScheduleDto>>(schedules)));
        }

        // POST: api/schedule
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<ScheduleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ScheduleResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateSchedule(CreateScheduleDto createDto)
        {
            var schedule = await _scheduleService.CreateScheduleAsync(createDto);
            return Ok(schedule);
        }

        // PUT: api/schedule/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateSchedule(Guid id, UpdateScheduleDto updateDto)
        {
            try
            {
                var result = await _scheduleService.UpdateScheduleAsync(id, updateDto);
                if (!result)
                    return NotFound($"Schedule with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/schedule/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
                return NotFound($"Schedule with ID {id} not found");

            return NoContent();
        }

        // POST: api/schedule/check-conflicts
        [HttpPost("check-conflicts")]
        public async Task<ActionResult<ScheduleConflictResult>> CheckConflicts(CheckScheduleDto checkDto)
        {
            var tempSchedule = new Schedule
            {
                LecturerId = checkDto.LecturerId,
                ClassroomId = checkDto.ClassroomId,
                Day = checkDto.Day,
                StartTime = checkDto.StartTime,
                EndTime = checkDto.EndTime
            };

            var conflicts = _conflictChecker.CheckAllConflicts(tempSchedule);
            return Ok(conflicts);
        }

        // GET: api/schedule/available-classrooms
        [HttpGet("available-classrooms")]
        public async Task<ActionResult<IEnumerable<ClassroomDto>>> GetAvailableClassrooms(
            [FromQuery] DayOfWeek day,
            [FromQuery] string startTime,
            [FromQuery] string endTime,
            [FromQuery] int? minCapacity = null)
        {
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            var availableClassrooms = _conflictChecker.GetAvailableClassrooms(day, start, end, minCapacity);
            return Ok(_mapper.Map<IEnumerable<ClassroomDto>>(availableClassrooms));
        }

        // GET: api/schedule/lecturer-available-slots/{lecturerId}
        [HttpGet("lecturer-available-slots/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetLecturerAvailableSlots(
            Guid lecturerId,
            [FromQuery] DayOfWeek day,
            [FromQuery] int durationMinutes = 60)
        {
            var slots = _conflictChecker.GetAvailableTimeSlotsForLecturer(lecturerId, day, durationMinutes);
            return Ok(slots);
        }
    }
}
