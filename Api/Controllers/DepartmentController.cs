using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class DepartmentController(IDepartmentService service) : BaseApiController
    {
        private readonly IDepartmentService _service = service;

        [HttpGet("get-departments")]
        [ProducesResponseType(typeof(BaseResponse<List<GetDepartmentsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<GetDepartmentsResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse<List<GetDepartmentsResponse>>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> GetDepartments([FromQuery] GetDepartmentRequest request)
        {
            return Ok(await _service.GetDepartments(request));
        }

        [HttpGet("get-faculties")]
        [ProducesResponseType(typeof(BaseResponse<List<GetFacultiesResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<GetFacultiesResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse<List<GetFacultiesResponse>>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> GetFaculties([FromQuery] GetFacultyRequest request)
        {
            return Ok(await _service.GetFaculties(request));
        }

        [HttpGet("{facultyId}/get-faculties")]
        [ProducesResponseType(typeof(BaseResponse<GetFacultiesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFacultiesResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse<GetFacultiesResponse>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> GetFacultyDetails(Guid facultyId)
        {
            return Ok(await _service.GetFacultyDetails(facultyId));
        }
    }
}
