using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Infrastructure.data;
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
            if(creatUserResponse == null || !creatUserResponse.Succeeded) return BaseResponse.Failure(500, "Failed to create user for student");
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
    }
}
