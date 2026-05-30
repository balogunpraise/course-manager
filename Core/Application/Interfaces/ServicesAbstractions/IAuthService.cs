using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IAuthService
    {
        Task<BaseResponse<Guid>> RegisterUserAsync(CreateUserRequest request);
        Task<BaseResponse<LogingResponse>> LoginAsync(LoginRequest request);
        Task<BaseResponse<GetUserResponse>> GetUserDetailsAsync(Guid userId);
        Task<BaseResponse> DeleteUserAccount(Guid userId);
    }
}
