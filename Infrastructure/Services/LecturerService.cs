using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Requests.Shared;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Core.Domain.Entities.LinkingEntities;
using Core.Domain.Enums;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class LecturerService(ApplicationDbContext context, 
        IAuthService authService, 
        ILogger<LecturerService> logger) : ILecturerService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<LecturerService> _logger = logger;

        public async Task<BaseResponse> CreateLecturerAsync(CreateLecturerRequest request)
        {
            // 1. Check for duplicate email before creating user account
            var emailExists = await _context.Lecturers
                .AnyAsync(l => l.User.Email == request.Email && !l.IsDeleted);

            if (emailExists)
                return BaseResponse.Failure(409, "A lecturer with this email already exists");

            // 2. Create user account
            var createUserRequest = new CreateUserRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = "DefaultPassword123!"
            };

            var createUserResponse = await _authService.RegisterUserAsync(createUserRequest, UserType.Lecturer);
            if (createUserResponse == null || !createUserResponse.Succeeded)
                return BaseResponse.Failure(500, "Failed to create user account for lecturer");

            var userId = createUserResponse.Data;

            // 3. Create lecturer profile
            try
            {
                var lecturer = new Lecturer
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Qualification = request.Qualification,
                    Specialization = request.Specialization,
                    Rank = request.Rank,
                    YearsOfExperience = request.YearsOfExperience,
                    IsAvailable = true,
                    UserId = userId,
                };

                // 4. Map preferred levels
                if (request.PrefferedLevels.Any())
                {
                    lecturer.PrefferedLevels = request.PrefferedLevels
                        .Select(l => new PreferedLevel
                        {
                            LevelId = l.LevelId
                        }).ToList();
                }

                // 5. Validate and map preferred courses
                if (request.PreferedCourses.Any())
                {
                    var requestedCourseIds = request.PreferedCourses.Select(c => c.CourseId).ToList();

                    var validCourseIds = await _context.Courses
                        .Where(c => requestedCourseIds.Contains(c.Id) && !c.IsDeleted)
                        .Select(c => c.Id)
                        .ToListAsync();

                    var invalidCourseIds = requestedCourseIds.Except(validCourseIds).ToList();
                    if (invalidCourseIds.Any())
                    {
                        await _authService.DeleteUserAccount(userId);
                        return BaseResponse.Failure(400, "One or more preferred courses are invalid",
                            invalidCourseIds.Select(id => $"Course {id} not found").ToList());
                    }

                    lecturer.PreferedCourses = validCourseIds
                        .Select(id => new PreferedCourse { CourseId = id })
                        .ToList();
                }

                await _context.Lecturers.AddAsync(lecturer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 6. Rollback user account if lecturer creation fails
                await _authService.DeleteUserAccount(userId);
                _logger.LogError(ex, "An error occurred while creating lecturer with email {Email}", request.Email);
                return BaseResponse.Failure(500, "An error occurred while creating the lecturer");
            }

            return BaseResponse.Success(201, "Lecturer created successfully");
        }



        public async Task<BaseResponse<LecturerResponse>> GetLecturerByIdAsync(Guid lecturerId)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.LecturerCourses)
                .Include(l => l.PrefferedLevels)
                .Include(l => l.PreferedCourses)
                .Where(l => l.Id == lecturerId && !l.IsDeleted)
                .Select(l => new LecturerResponse
                {
                    Id = l.Id,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    MiddleName = l.MiddleName,
                    Email = l.User.Email,
                    Qualification = l.Qualification,
                    Specialization = l.Specialization,
                    Rank = l.Rank,
                    YearsOfExperience = l.YearsOfExperience,
                    MaxCourseLoad = l.MaxCourseLoad,
                    IsAvailable = l.IsAvailable,
                    AllocatedCoursesCount = l.LecturerCourses.Count,
                    PreferredLevels = l.PrefferedLevels.Select(p => p.Level.ToString()).ToList(),
                    PreferredCourses = l.PreferedCourses.Select(p => p.Course.CourseCode).ToList()
                })
                .FirstOrDefaultAsync();

            if (lecturer == null)
                return BaseResponse<LecturerResponse>.Failure(404, "Lecturer not found");

            return BaseResponse<LecturerResponse>.Success(200, "Lecturer retrieved successfully", lecturer);
        }

        public async Task<BaseResponse<PagedList<LecturerResponse>>> GetAllLecturersAsync(RequestParams request)
        {
            var query = _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.LecturerCourses)
                .Where(l => !l.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower().Trim();
                query = query.Where(l =>
                    l.FirstName.ToLower().Contains(term) ||
                    l.LastName.ToLower().Contains(term) ||
                    l.MiddleName.ToLower().Contains(term) ||
                    l.User.Email.ToLower().Contains(term));
            }

            
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    var parts = filter.Split(':');
                    if (parts.Length != 2) continue;

                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim().ToLower();

                    query = key switch
                    {
                        "specialization" when Enum.TryParse<Specialization>(value, true, out var spec)
                            => query.Where(l => l.Specialization == spec),

                        "qualification" when Enum.TryParse<Qualification>(value, true, out var qual)
                            => query.Where(l => l.Qualification == qual),

                        "rank" when Enum.TryParse<AcademicRank>(value, true, out var rank)
                            => query.Where(l => l.Rank == rank),

                        "isavailable" when bool.TryParse(value, out var available)
                            => query.Where(l => l.IsAvailable == available),

                        _ => query
                    };
                }
            }

            
            query = request.Sort?.SortValue?.ToLower() switch
            {
                "firstname" => request.Sort.SortOrder == "DESC"
                                        ? query.OrderByDescending(l => l.FirstName)
                                        : query.OrderBy(l => l.FirstName),

                "lastname" => request.Sort.SortOrder == "DESC"
                                        ? query.OrderByDescending(l => l.LastName)
                                        : query.OrderBy(l => l.LastName),

                "rank" => request.Sort.SortOrder == "DESC"
                                        ? query.OrderByDescending(l => l.Rank)
                                        : query.OrderBy(l => l.Rank),

                "yearsofexperience" => request.Sort.SortOrder == "DESC"
                                        ? query.OrderByDescending(l => l.YearsOfExperience)
                                        : query.OrderBy(l => l.YearsOfExperience),

                _ => request.Sort?.SortOrder == "DESC"   
                                        ? query.OrderByDescending(l => l.CreatedAt)
                                        : query.OrderBy(l => l.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var projectedQuery = query.Select(l => new LecturerResponse
            {
                Id = l.Id,
                FirstName = l.FirstName,
                LastName = l.LastName,
                MiddleName = l.MiddleName,
                Email = l.User.Email,
                Qualification = l.Qualification,
                Specialization = l.Specialization,
                Rank = l.Rank,
                YearsOfExperience = l.YearsOfExperience,
                MaxCourseLoad = l.MaxCourseLoad,
                IsAvailable = l.IsAvailable,
                AllocatedCoursesCount = l.LecturerCourses.Count
            });

            var pagedResult = await PagedList<LecturerResponse>.ToPagedListAsync(
                projectedQuery, request.PageSize, request.PageNumber, totalCount);

            return BaseResponse<PagedList<LecturerResponse>>.Success(
                200, $"{totalCount} lecturer(s) found", pagedResult);
        }


        public async Task<BaseResponse> UpdateLecturerAsync(Guid lecturerId, UpdateLecturerRequest request)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.PrefferedLevels)
                .Include(l => l.PreferedCourses)
                .FirstOrDefaultAsync(l => l.Id == lecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            lecturer.FirstName = request.FirstName;
            lecturer.LastName = request.LastName;
            lecturer.MiddleName = request.MiddleName;
            lecturer.Qualification = request.Qualification;
            lecturer.Specialization = request.Specialization;
            lecturer.Rank = request.Rank;
            lecturer.YearsOfExperience = request.YearsOfExperience;
            lecturer.MaxCourseLoad = request.MaxCourseLoad;

            // Replace preferred levels and courses
            _context.RemoveRange(lecturer.PrefferedLevels);
            _context.RemoveRange(lecturer.PreferedCourses);

            lecturer.PrefferedLevels = request.PrefferedLevels ?? new List<PreferedLevel>();
            lecturer.PreferedCourses = request.PreferedCourses ?? new List<PreferedCourse>();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating lecturer {LecturerId}", lecturerId);
                return BaseResponse.Failure(500, "An error occurred while updating the lecturer");
            }

            return BaseResponse.Success(200, "Lecturer updated successfully");
        }


        public async Task<BaseResponse> ToggleLecturerAvailabilityAsync(Guid lecturerId)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            lecturer.IsAvailable = !lecturer.IsAvailable;
            await _context.SaveChangesAsync();

            var status = lecturer.IsAvailable ? "available" : "unavailable";
            return BaseResponse.Success(200, $"{lecturer.FirstName} {lecturer.LastName} is now marked as {status}");
        }


        public async Task<BaseResponse> DeleteLecturerAsync(Guid lecturerId)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            lecturer.IsDeleted = true;

            try
            {
                await _authService.DeleteUserAccount(lecturer.UserId);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting lecturer {LecturerId}", lecturerId);
                return BaseResponse.Failure(500, "An error occurred while deleting the lecturer");
            }

            return BaseResponse.Success(200, "Lecturer deleted successfully");
        }
    }
}
