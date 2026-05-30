using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IStudentService
    {
        Task<BaseResponse> CreateStudentAsync(CreateStudentRequest request);
    }
}
