using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class StudentService(ApplicationDbContext context,
        IAuthService authService,
        ILogger<StudentService> logger) : IStudentService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<StudentService> _logger = logger;

        public async Task<BaseResponse> CreateStudentAsync(CreateStudentRequest request)
        {
            var createUserRequest = new CreateUserRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                Password = "DefaultPassword123!"
            };
            var creatUserResponse = await _authService.RegisterUserAsync(createUserRequest);
            if (creatUserResponse == null || !creatUserResponse.Succeeded) return BaseResponse.Failure(500, "Failed to create user for student");
            var userId = creatUserResponse.Data;

            try
            {
                var addedStudent = _context.Students.AddAsync(new Student
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Email = request.Email,
                    DepartmentId = request.DepartmentId,
                    LevelId = request.LevelId,
                    UserId = userId
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _authService.DeleteUserAccount(userId);
                _logger.LogError(ex, "An error occurred while creating a student with email {Email}", request.Email);
                return BaseResponse.Failure(500, "An error occurred while creating the student", new List<string> { ex.Message });
            }
            return BaseResponse.Success(201, "Student created successfully");
        }

        public async Task<BaseResponse<PagedList<ListStudentResponses>>> ListStudentsAsync(ListStudentRequest request)
        {
            var query = _context.Students
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    s.Email.ToLower().Contains(searchTerm) ||
                    s.StudentNumber.ToLower().Contains(searchTerm));
            }

            var count = await query.CountAsync();

            var students = query
                .OrderBy(x => x.FirstName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new ListStudentResponses
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    MiddleName = s.MiddleName,
                    Email = s.Email,
                    StudentNumber = s.StudentNumber,
                    DateOfBirth = s.DateOfBirth,
                    EnrollmentDate = s.EnrollmentDate,
                    Status = s.Status.ToString(),
                    GPA = s.GPA,
                    Department = s.Department.Name,
                    Level = s.Level.LevelName,
                    DepartmentId = s.DepartmentId,
                    LevelId = s.LevelId
                });

            var pagedStudents = await PagedList<ListStudentResponses>
                .ToPagedListAsync(students, request.PageNumber, request.PageSize, count);

            return BaseResponse<PagedList<ListStudentResponses>>
                .Success(200, "Students retrieved successfully", pagedStudents);
        }


        public async Task<BaseResponse<GetStudentDetailsResponse>> GetStudentByIdAsync(Guid studentId)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Transcripts)
                .Include(s => s.WaitlistEntries)
                .Where(x => x.Id == studentId && !x.IsDeleted)
                .Select(s => new GetStudentDetailsResponse
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    MiddleName = s.MiddleName,
                    Email = s.Email,
                    StudentNumber = s.StudentNumber,
                    DateOfBirth = s.DateOfBirth,
                    EnrollmentDate = s.EnrollmentDate,
                    Status = s.Status.ToString(),
                    GPA = s.GPA,
                    Department = s.Department.Name,
                    Level = s.Level.LevelName,
                    DepartmentId = s.DepartmentId,
                    LevelId = s.LevelId,
                    Enrollments = s.Enrollments,
                    Transcripts = s.Transcripts,
                    WaitlistEntries = s.WaitlistEntries,
                })
                .FirstOrDefaultAsync();
            if (student == null) return BaseResponse<GetStudentDetailsResponse>.Failure(404, "Student not found");
            return BaseResponse<GetStudentDetailsResponse>.Success(200, "Student retrieved successfully", student);
        }

        public async Task<BaseResponse> DeleteStudentAsync(Guid studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null || student.IsDeleted) return BaseResponse.Failure(404, "Student not found");
            student.IsDeleted = true;
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Student deleted successfully");
        }

        public async Task<BaseResponse> UpdateStudentAsync(Guid studentId, CreateStudentRequest request)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null || student.IsDeleted) return BaseResponse.Failure(404, "Student not found");
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.MiddleName = request.MiddleName;
            student.DepartmentId = request.DepartmentId;
            student.LevelId = request.LevelId;
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Student updated successfully");
        }

        public async Task<BaseResponse> EnrolCourses(EnrollStudentRequest request)
        {
            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null || student.IsDeleted) return BaseResponse.Failure(404, "Student not found");
            var courses = await _context.Courses.Where(c => request.CourseIds.Contains(c.Id)).ToListAsync();
            if (courses.Count != request.CourseIds.Count) return BaseResponse.Failure(404, "One or more courses not found");
            foreach (var course in courses)
            {
                //if (course.Capacity <= course.Enrollments.Count)
                //{
                //    return BaseResponse.Failure(400, $"Course {course.CourseCode} is full");
                //}
                if (student.Enrollments.Any(e => e.CourseId == course.Id))
                {
                    return BaseResponse.Failure(400, $"Student is already enrolled in course {course.CourseCode}");
                }
                student.Enrollments.Add(new Enrollment
                {
                    CourseId = course.Id,
                    StudentId = request.StudentId,
                    EnrolledAt = DateTime.UtcNow,
                    EnrollmentStatus = EnrollmentStatus.Enrolled
                });
            }
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Courses enrolled successfully");
        }
        public async Task<BaseResponse> DropCourse(Guid studentId, Guid courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.DroppedAt == null);
            if (enrollment == null) return BaseResponse.Failure(404, "Enrollment not found");
            enrollment.DroppedAt = DateTime.UtcNow;
            enrollment.EnrollmentStatus = EnrollmentStatus.Dropped;
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Course dropped successfully");
        }
    }
}
