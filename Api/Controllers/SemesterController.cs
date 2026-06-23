using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class SemesterController(ISemesterService semesterService) : BaseApiController
    {
        private readonly ISemesterService _semesterService = semesterService;


        [HttpPost("sessions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddAcademicSession(SessionRequest request)
        {
            return Ok(await _semesterService.CreateAcademicSession(request));
        }

        [HttpGet("sessions")]
        [ProducesResponseType(typeof(BaseResponse<List<SessionResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<SessionResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAcademicSessions()
        {
            return Ok(await _semesterService.GetAcademicSessions());
        }
    }
}
