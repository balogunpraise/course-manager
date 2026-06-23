using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ISemesterService
    {
        Task<BaseResponse<bool>> CreateAcademicSession(SessionRequest request);
        Task<BaseResponse<List<SessionResponse>>> GetAcademicSessions();
    }
}
