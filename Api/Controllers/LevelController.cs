using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class LevelController(ILevelService levelService) : BaseApiController
    {
        private readonly ILevelService _levelService = levelService;


        [HttpGet("get-levels")]
        [ProducesResponseType(typeof(BaseResponse<List<GetLevelsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<GetLevelsResponse>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<List<GetLevelsResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetLevels([FromQuery] GetLevelsRequests request)
        {
            var response = await _levelService.GetLevels(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }
    }
}
