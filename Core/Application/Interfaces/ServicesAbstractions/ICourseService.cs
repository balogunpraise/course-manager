using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ICourseService
    {
        Task<BaseResponse> CreateCourseAsync(CreateCourseRequest request);
        Task<BaseResponse<GetCourseResponse>> GetCourseByIdAsync(Guid courseId);
        Task<BaseResponse> UpdateCourseAsync(UpdateCourseRequest request);
        Task<BaseResponse> DeleteCourseAsync(Guid courseId);
        Task<BaseResponse<PagedList<GetCourseResponse>>> GetAllCoursesAsync(ListCoursesRequest request);
    }
}
