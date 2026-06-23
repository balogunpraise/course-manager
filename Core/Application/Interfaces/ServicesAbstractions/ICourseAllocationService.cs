using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ICourseAllocationService
    {
        Task<Core.Application.Dtos.Responses.BaseResponse> AllocateCoursesManually(Core.Application.Dtos.Requests.ManualCourseAllocationRequest request);
        Task<BaseResponse> ReassignCourse(ReassignCourseRequest request);
        Task<BaseResponse<List<AllocatedCourseResponse>>> GetLecturerAllocatedCourses(Guid lecturerId);
        Task<BaseResponse> BulkRemoveCoursesFromLecturer(RemoveCoursesBulkRequest request);
        Task<BaseResponse> RemoveCoursesFromLecturer(RemoveCoursesBulkRequest request);
        Task<BaseResponse<List<AllocatedCourseResponse>>> LecturerGetAssignedCourses(Guid userId);

    }
}
