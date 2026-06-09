using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class StudentController(IStudentService studentService) : BaseApiController
    {
        private readonly IStudentService _studentService = studentService;

        [HttpPost("create-student")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateStudent([FromBody] CreateStudentRequest request)
        {
            var response = await _studentService.CreateStudentAsync(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }


        [HttpGet("get-students")]
        [ProducesResponseType(typeof(BaseResponse<PagedList<ListStudentResponses>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<PagedList<ListStudentResponses>>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ListStudents([FromQuery] ListStudentRequest request)
        {
            var response = await _studentService.ListStudentsAsync(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }


        [HttpGet("get-student-details")]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetStudentDetails(Guid studentId)
        {
            var response = await _studentService.GetStudentByIdAsync(studentId);
            if (!response.Succeeded) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-student-self-record")]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<GetStudentDetailsResponse>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> StudentSelfRecord()
        {
            var studentId = LoggedInUserId();
            var response = await _studentService.GetStudentByIdAsync(studentId);
            if (!response.Succeeded) return NotFound(response);
            return Ok(response);
        }

        //public async Task<ActionResult> UpdateStudent([FromBody] UpdateStudentRequest request)
        //{
        //    var response = await _studentService.UpdateStudentAsync(request);
        //    if (!response.Succeeded) return BadRequest(response);
        //    return Ok(response);
        //}

        [HttpDelete("delete-student")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> DeleteStudent(Guid studentId)
        {
            var response = await _studentService.DeleteStudentAsync(studentId);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }


        [HttpPost("enroll-courses")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EnrollCourses([FromBody] EnrollStudentRequest request)
        {
            var response = await _studentService.EnrolCourses(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("self-enroll-course")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> SelfEnrollCourse([FromBody] SelfEnrollCourseRequest request)
        {
            var studentId = LoggedInUserId();
            var enrollRequest = new EnrollStudentRequest
            {
                StudentId = studentId,
                CourseIds = request.CourseIds
            };
            var response = await _studentService.EnrolCourses(enrollRequest);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }
    }
}
