using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IStudentService
    {
        Task<BaseResponse> CreateStudentAsync(CreateStudentRequest request);
        Task<BaseResponse<PagedList<ListStudentResponses>>> ListStudentsAsync(ListStudentRequest request);
        Task<BaseResponse<GetStudentDetailsResponse>> GetStudentByIdAsync(Guid studentId);
        Task<BaseResponse> DeleteStudentAsync(Guid studentId);
        Task<BaseResponse> UpdateStudentAsync(Guid studentId, CreateStudentRequest request);
        Task<BaseResponse> EnrolCourses(EnrollStudentRequest request);
        Task<BaseResponse> DropCourse(Guid studentId, Guid courseId);

    }
}
