using AutoMapper;
using Core.Application.Dtos;
using Core.Application.Dtos.Responses;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassroomController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClassroomController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/classroom
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ClassroomDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ClassroomDto>>), StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClassroomDto>>> GetAllClassrooms(
            [FromQuery] ClassroomType? type = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] int? minCapacity = null)
        {
            var query = _context.Classrooms.Where(c => !c.IsDeleted);

            if (type.HasValue)
                query = query.Where(c => c.Type == type.Value);

            if (isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable.Value);

            if (minCapacity.HasValue)
                query = query.Where(c => c.Capacity >= minCapacity.Value);

            var classrooms = await query.ToListAsync();
            return Ok(BaseResponse<IEnumerable<ClassroomDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ClassroomDto>>(classrooms)));
        }

        // GET: api/classroom/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<ClassroomDto>> GetClassroom(Guid id)
        {
            var classroom = await _context.Classrooms
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (classroom == null)
                return Ok(BaseResponse<ClassroomDto>.Failure(StatusCodes.Status404NotFound, $"Classroom with ID {id} not found"));

            return Ok(BaseResponse<ClassroomDto>.Success(StatusCodes.Status200OK, "success", _mapper.Map<ClassroomDto>(classroom)));
        }

        // POST: api/classroom
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateClassroom(CreateClassroomDto createDto)
        {
            var classroom = _mapper.Map<Classroom>(createDto);
            classroom.Id = Guid.NewGuid();
            classroom.IsAvailable = true;

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            return Ok(BaseResponse<ClassroomDto>.Success(StatusCodes.Status200OK, "succes", _mapper.Map<ClassroomDto>(classroom)));
        }

        // PUT: api/classroom/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClassroom(Guid id, UpdateClassroomDto updateDto)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null || classroom.IsDeleted)
                return Ok(BaseResponse.Failure(StatusCodes.Status404NotFound, $"Classroom with ID {id} not found"));

            _mapper.Map(updateDto, classroom);
            await _context.SaveChangesAsync();

            return Ok(BaseResponse.Success(StatusCodes.Status204NoContent, "success"));
        }

        // DELETE: api/classroom/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteClassroom(Guid id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null || classroom.IsDeleted)
                return NotFound($"Classroom with ID {id} not found");

            // Check if classroom has any future schedules
            var hasFutureSchedules = await _context.Schedules
                .AnyAsync(s => s.ClassroomId == id && !s.IsDeleted);

            if (hasFutureSchedules)
                return Ok(BaseResponse.Failure(StatusCodes.Status400BadRequest, "Cannot delete classroom with existing schedules"));

            classroom.IsDeleted = true;
            classroom.IsAvailable = false;
            await _context.SaveChangesAsync();

            return Ok(BaseResponse.Success(StatusCodes.Status204NoContent, "success"));
        }

        // GET: api/classroom/{id}/schedule
        [HttpGet("{id}/schedule")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status500InternalServerError)]
        
        [AllowAnonymous]
        public async Task<ActionResult> GetClassroomSchedule(
            Guid id,
            [FromQuery] DayOfWeek? day = null)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null || classroom.IsDeleted)
                return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Failure(StatusCodes.Status404NotFound, $"Classroom with ID {id} not found"));

            var query = _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Where(s => s.ClassroomId == id && !s.IsDeleted);

            if (day.HasValue)
                query = query.Where(s => s.Day == day.Value);

            var schedules = await query
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(BaseResponse<IEnumerable<ScheduleDto>>.Success(StatusCodes.Status200OK, "success", _mapper.Map<IEnumerable<ScheduleDto>>(schedules)));
        }
    }
}
