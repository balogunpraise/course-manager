using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CourseService(ApplicationDbContext context) : ICourseService
    {
        private readonly ApplicationDbContext _context = context;


        public async Task<BaseResponse> CreateCourseAsync(CreateCourseRequest request)
        {
            var isCourseCodeExists = await _context.Courses.AnyAsync(c => c.CourseCode == request.CourseCode);
            if (isCourseCodeExists) return BaseResponse.Failure(400, "Course code already exists");

            var course = new Course
            {
                CourseCode = request.CourseCode,
                Title = request.Title,
                Description = request.Description,
                CreditHours = request.CreditHours
            };
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return BaseResponse.Success(201, "Course created successfully");
        }


        public async Task<BaseResponse<GetCourseResponse>> GetCourseByIdAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return BaseResponse<GetCourseResponse>.Failure(404, "Course not found");
            var response = new GetCourseResponse
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                CreditHours = course.CreditHours,
                DepartmentId = course.DepartmentId.GetValueOrDefault(),
                Department = course.Department,
            };
            return BaseResponse<GetCourseResponse>.Success(200, "Course retrieved successfully", response);
        }

        public async Task<BaseResponse> UpdateCourseAsync(UpdateCourseRequest request)
        {
            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null) return BaseResponse.Failure(404, "Course not found");
            if (course.CourseCode != request.CourseCode)
            {
                var isCourseCodeExists = await _context.Courses.AnyAsync(c => c.CourseCode == request.CourseCode && c.Id != request.CourseId);
                if (isCourseCodeExists) return BaseResponse.Failure(400, "Course code already exists");
            }
            course.CourseCode = request.CourseCode;
            course.Title = request.Title;
            course.Description = request.Description;
            course.CreditHours = request.CreditHours;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Course updated successfully");
        }

        public async Task<BaseResponse> DeleteCourseAsync(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return BaseResponse.Failure(404, "Course not found");
            course.IsDeleted = true;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return BaseResponse.Success(200, "Course deleted successfully");
        }


        public async Task<BaseResponse<PagedList<GetCourseResponse>>> GetAllCoursesAsync(ListCoursesRequest request)
        {
            var query = _context.Courses
                .Include(c => c.Department)
                .Where(c => !c.IsDeleted)
                .AsQueryable();
            if(!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c => c.CourseCode.Contains(request.SearchTerm) || c.Title.Contains(request.SearchTerm));
            }
            var totalCount = await query.CountAsync();
            var courses = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            var response = courses.Select(course => new GetCourseResponse
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                CreditHours = course.CreditHours,
                DepartmentId = course.DepartmentId.GetValueOrDefault(),
                Department = course.Department,
            }).ToList();
            return BaseResponse<PagedList<GetCourseResponse>>.Success(200, "Courses retrieved successfully", PagedList<GetCourseResponse>.ToPagedList(response, request.PageSize, request.PageNumber, totalCount));
        }
    }
}
