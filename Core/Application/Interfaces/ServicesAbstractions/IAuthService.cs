using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Domain.Enums;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IAuthService
    {
        Task<BaseResponse<Guid>> RegisterUserAsync(CreateUserRequest request, UserType userType = UserType.Student);
        Task<BaseResponse<LogingResponse>> LoginAsync(LoginRequest request);
        Task<BaseResponse<GetUserResponse>> GetUserDetailsAsync(Guid userId);
        Task<BaseResponse> DeleteUserAccount(Guid userId);
    }
}
