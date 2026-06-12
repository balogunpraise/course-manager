using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseAllocationController(ICourseAllocationService allocationService) : BaseApiController
    {
        private readonly ICourseAllocationService _allocationService = allocationService;

        /// <summary>Manually allocates one or more courses to a lecturer</summary>
        [HttpPost("manual")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AllocateCoursesManually([FromBody] ManualCourseAllocationRequest request)
        {
            var response = await _allocationService.AllocateCoursesManually(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Gets all courses allocated to a specific lecturer</summary>
        [HttpGet("lecturer/{lecturerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<List<AllocatedCourseResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLecturerAllocatedCourses([FromRoute] Guid lecturerId)
        {
            var response = await _allocationService.GetLecturerAllocatedCourses(lecturerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Reassigns a course from one lecturer to another</summary>
        [HttpPatch("reassign")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReassignCourse([FromBody] ReassignCourseRequest request)
        {
            var response = await _allocationService.ReassignCourse(request);
            return StatusCode(response.StatusCode, response);
        }

        ///// <summary>Removes a single course from a lecturer</summary>
        //[HttpDelete]
        //[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> RemoveCourseFromLecturer([FromBody] RemoveCourseAllocationRequest request)
        //{
        //    var response = await _allocationService.RemoveCoursesFromLecturer(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        /// <summary>Removes multiple courses from a lecturer at once</summary>
        [HttpDelete("remove-courses-from-lecturer")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCoursesFromLecturerBulk([FromBody] RemoveCoursesBulkRequest request)
        {
            var response = await _allocationService.RemoveCoursesFromLecturer(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
