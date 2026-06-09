

using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ILevelService
    {
        Task<BaseResponse<List<GetLevelsResponse>>> GetLevels(GetLevelsRequests request);
    }
}
