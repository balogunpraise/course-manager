using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class DepartmentService(ApplicationDbContext context, ILogger<DepartmentService> logger) : IDepartmentService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<DepartmentService> _logger = logger;


        public async Task<BaseResponse<List<GetDepartmentsResponse>>> GetDepartments(GetDepartmentRequest request)
        {
            var query = _context.Departments.AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }
            var departments = await query.ToListAsync();
            var response = departments.Select(x => new GetDepartmentsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                FacultyId = x.FacultyId
            }).ToList();
            return BaseResponse<List<GetDepartmentsResponse>>.Success(200, "success", response);
        }

        public async Task<BaseResponse<List<GetFacultiesResponse>>> GetFaculties(GetFacultyRequest request)
        {
            var query = _context.Faculties.AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }
            var faculties = await query.ToListAsync();
            var response = faculties.Select(x => new GetFacultiesResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            return BaseResponse<List<GetFacultiesResponse>>.Success(200, "success", response);
        }

        public async Task<BaseResponse<GetFacultiesResponse>> GetFacultyDetails(Guid facultyId)
        {
            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty is null) return BaseResponse<GetFacultiesResponse>.Failure();
            var response = new GetFacultiesResponse
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Description = faculty.Description
            };
            return BaseResponse<GetFacultiesResponse>.Success(200, "success", response);
        }
    }
}
