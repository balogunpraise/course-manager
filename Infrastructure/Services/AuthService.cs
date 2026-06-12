using Core.Application.Constants;
using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Helpers;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities.Auth;
using Core.Domain.Enums;
using Infrastructure.data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AuthService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ApplicationDbContext context,
        ITokenService tokenService
    ) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<AppRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;


        public async Task<BaseResponse<Guid>> RegisterUserAsync(CreateUserRequest request, UserType userType = UserType.Student)
        {
            var useEmailExists = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (useEmailExists != null)
            {
                return BaseResponse<Guid>.Failure(400, "Email already exists");
            }
            var user = new AppUser
            {
                UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email.Trim() : request.UserName.Trim(),
                Email = request.Email.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim()
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // Assign default role to the user
                var defaultRole = GetDefaultRoleForUserType(userType); // You can change this to your desired default role
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = defaultRole });
                }
                await _userManager.AddToRoleAsync(user, defaultRole);
                return BaseResponse<Guid>.Success(200, "User registered successfully", user.Id);
            }
            return BaseResponse<Guid>.Failure(400, "User registration failed", result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<BaseResponse<LogingResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BaseResponse<LogingResponse>.Failure(404, "Invalid Credentials");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BaseResponse<LogingResponse>.Failure(400, "Invalid Credentials");
            }
            var token = await _tokenService.GenerateJwtToken(user);
            var response = new LogingResponse
            {
                UserId = user.Id,
                Token = token,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ""
            };
            return BaseResponse<LogingResponse>.Success(200, "User logged in successfully", response);
        }

        public async Task<BaseResponse<GetUserResponse>> GetUserDetailsAsync(Guid userId)
        {
            var user = await _userManager.GetUserById(userId);
            if (user == null)
            {
                return BaseResponse<GetUserResponse>.Failure(404, "User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var response = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };
            return BaseResponse<GetUserResponse>.Success(200, "User retrieved successfully", response);
        }

        public async Task<BaseResponse> DeleteUserAccount(Guid userId)
        {
            var user = await _userManager.GetUserById(userId);
            if (user == null)
            {
                return BaseResponse.Failure(404, "User not found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return BaseResponse.Success(200, "User account deleted successfully");
            }
            return BaseResponse.Failure(400, "Failed to delete user account", result.Errors.Select(e => e.Description).ToList());
        }

        private string GetDefaultRoleForUserType(UserType userType)
        {
            return userType switch
            {
                UserType.Student => RoleConstants.Student,
                UserType.Lecturer => RoleConstants.Lecturer,
                UserType.Admin => RoleConstants.Admin,
                _ => RoleConstants.Student
            };
        }
    }
}
