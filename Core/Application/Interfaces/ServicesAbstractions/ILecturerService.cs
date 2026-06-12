using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Requests.Shared;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ILecturerService
    {
        Task<BaseResponse> CreateLecturerAsync(CreateLecturerRequest request);
        Task<BaseResponse<LecturerResponse>> GetLecturerByIdAsync(Guid lecturerId);
        Task<BaseResponse<PagedList<LecturerResponse>>> GetAllLecturersAsync(RequestParams request);
        Task<BaseResponse> UpdateLecturerAsync(Guid lecturerId, UpdateLecturerRequest request);
        Task<BaseResponse> ToggleLecturerAvailabilityAsync(Guid lecturerId);
        Task<BaseResponse> DeleteLecturerAsync(Guid lecturerId);
    }
}
