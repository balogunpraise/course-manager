using AutoMapper;
using Core.Application.Dtos;
using Core.Application.Dtos.Responses;
using Core.Domain.Entities;
using Infrastructure.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    public class LecturerAvailabilityController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LecturerAvailabilityController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/lectureravailability/lecturer/{lecturerId}
        [HttpGet("lecturer/{lecturerId}")]
        [ProducesResponseType(typeof(BaseResponse<List<LecturerAvailabilityDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<LecturerAvailabilityDto>>), StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult> GetLecturerAvailabilities(
            Guid lecturerId,
            [FromQuery] DayOfWeek? day = null)
        {
            var query = _context.LecturerAvailabilities
                .Where(a => a.LecturerId == lecturerId && !a.IsDeleted);

            if (day.HasValue)
                query = query.Where(a => a.Day == day.Value);

            var availabilities = await query
                .OrderBy(a => a.Day)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
            var mappedResponse = _mapper.Map<List<LecturerAvailabilityDto>>(availabilities);
            var response = BaseResponse<List<LecturerAvailabilityDto>>.Success(StatusCodes.Status200OK, "success", mappedResponse);
            return Ok(response);
        }

        // GET: api/lectureravailability/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<LecturerAvailabilityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<LecturerAvailabilityDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAvailability(Guid id)
        {
            var availability = await _context.LecturerAvailabilities
                .Include(a => a.Lecturer)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (availability == null)
                return Ok(BaseResponse<LecturerAvailabilityDto>.Failure(StatusCodes.Status404NotFound, $"Availability record with ID {id} not found"));
            var mapped = _mapper.Map<LecturerAvailabilityDto>(availability);
            return Ok(BaseResponse<LecturerAvailabilityDto>.Success(StatusCodes.Status200OK, "success", mapped));
        }

        // POST: api/lectureravailability
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<LecturerAvailabilityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<LecturerAvailabilityDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAvailability(CreateLecturerAvailabilityDto createDto)
        {
            // Verify lecturer exists
            var lecturer = await _context.Lecturers.FindAsync(createDto.LecturerId);
            if (lecturer == null)
                return Ok(BaseResponse<LecturerAvailabilityDto>.Failure(StatusCodes.Status400BadRequest, $"Lecturer with ID {createDto.LecturerId} not found"));

            // Check for overlapping availability periods
            var overlapping = await _context.LecturerAvailabilities
                .AnyAsync(a => a.LecturerId == createDto.LecturerId &&
                              a.Day == createDto.Day &&
                              !a.IsDeleted &&
                              !(createDto.EndTime <= a.StartTime || createDto.StartTime >= a.EndTime));

            if (overlapping)
                Ok(BaseResponse<LecturerAvailabilityDto>.Failure(StatusCodes.Status400BadRequest, $"Availability period overlaps with existing availability record"));

            var availability = _mapper.Map<LecturerAvailability>(createDto);
            availability.Id = Guid.NewGuid();

            _context.LecturerAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return Ok(BaseResponse<LecturerAvailabilityDto>.Success(StatusCodes.Status200OK, "success", 
                _mapper.Map<LecturerAvailabilityDto>(availability)));
        }

        // POST: api/lectureravailability/batch
        [HttpPost("batch")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBatchAvailability(CreateBatchAvailabilityDto batchDto)
        {
            var lecturer = await _context.Lecturers.FindAsync(batchDto.LecturerId);
            if (lecturer == null)
                return BadRequest($"Lecturer with ID {batchDto.LecturerId} not found");

            var availabilities = new List<LecturerAvailability>();

            foreach (var day in batchDto.Days)
            {
                foreach (var timeSlot in batchDto.TimeSlots)
                {
                    // Check for overlap
                    var overlapping = await _context.LecturerAvailabilities
                        .AnyAsync(a => a.LecturerId == batchDto.LecturerId &&
                                      a.Day == day &&
                                      !a.IsDeleted &&
                                      !(timeSlot.EndTime <= a.StartTime || timeSlot.StartTime >= a.EndTime));

                    if (!overlapping)
                    {
                        availabilities.Add(new LecturerAvailability
                        {
                            Id = Guid.NewGuid(),
                            LecturerId = batchDto.LecturerId,
                            Day = day,
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime,
                            IsAvailable = true
                        });
                    }
                }
            }

            if (availabilities.Any())
            {
                await _context.LecturerAvailabilities.AddRangeAsync(availabilities);
                await _context.SaveChangesAsync();
            }

            return Ok(new { created = availabilities.Count, totalRequested = batchDto.Days.Count * batchDto.TimeSlots.Count });
        }

        // PUT: api/lectureravailability/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAvailability(Guid id, UpdateLecturerAvailabilityDto updateDto)
        {
            var availability = await _context.LecturerAvailabilities.FindAsync(id);
            if (availability == null || availability.IsDeleted)
                return Ok(BaseResponse.Failure(StatusCodes.Status404NotFound));

            // Check for overlap with other availability records
            var overlapping = await _context.LecturerAvailabilities
                .AnyAsync(a => a.LecturerId == availability.LecturerId &&
                              a.Day == updateDto.Day &&
                              a.Id != id &&
                              !a.IsDeleted &&
                              !(updateDto.EndTime <= a.StartTime || updateDto.StartTime >= a.EndTime));

            if (overlapping)
                return Ok(BaseResponse.Failure());

            _mapper.Map(updateDto, availability);
            await _context.SaveChangesAsync();

            return Ok(BaseResponse.Success(StatusCodes.Status204NoContent, "success"));
        }

        // DELETE: api/lectureravailability/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAvailability(Guid id)
        {
            var availability = await _context.LecturerAvailabilities.FindAsync(id);
            if (availability == null || availability.IsDeleted)
                return Ok(BaseResponse.Failure(StatusCodes.Status404NotFound));

            availability.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok(BaseResponse.Success(StatusCodes.Status200OK, "success"));
        }

        // DELETE: api/lectureravailability/lecturer/{lecturerId}/day/{day}
        [HttpDelete("lecturer/{lecturerId}/day/{day}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDayAvailabilities(Guid lecturerId, DayOfWeek day)
        {
            var availabilities = await _context.LecturerAvailabilities
                .Where(a => a.LecturerId == lecturerId && a.Day == day && !a.IsDeleted)
                .ToListAsync();

            foreach (var availability in availabilities)
            {
                availability.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return Ok(BaseResponse.Success(StatusCodes.Status200OK, "success"));
        }
    }
}
