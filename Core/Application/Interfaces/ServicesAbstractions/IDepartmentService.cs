using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface IDepartmentService
    {
        Task<BaseResponse<List<GetDepartmentsResponse>>> GetDepartments(GetDepartmentRequest request);
        Task<BaseResponse<List<GetFacultiesResponse>>> GetFaculties(GetFacultyRequest request);
        Task<BaseResponse<GetFacultiesResponse>> GetFacultyDetails(Guid facultyId);
    }
}
