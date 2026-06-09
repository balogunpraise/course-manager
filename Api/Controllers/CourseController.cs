using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class CourseController(ICourseService courseService) : BaseApiController
    {
        private readonly ICourseService _courseService = courseService;
        [HttpPost("create-course")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var response = await _courseService.CreateCourseAsync(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }
        [HttpGet("get-course-details")]
        [ProducesResponseType(typeof(BaseResponse<GetCourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetCourseResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<GetCourseResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCourseDetails(Guid courseId)
        {
            var response = await _courseService.GetCourseByIdAsync(courseId);
            if (!response.Succeeded) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-all-courses")]
        [ProducesResponseType(typeof(BaseResponse<PagedList<GetCourseResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<PagedList<GetCourseResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse<PagedList<GetCourseResponse>>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllCounses([FromQuery] ListCoursesRequest request)
        {
            var response = await _courseService.GetAllCoursesAsync(request);
            return Ok(response);
        }
    }
}
