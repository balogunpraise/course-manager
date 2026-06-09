using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AuthController(IAuthService authService) : BaseApiController
    {
        private readonly IAuthService _authService = authService;


        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] CreateUserRequest request)
        {
            var response = await _authService.RegisterUserAsync(request);
            if (!response.Succeeded) return BadRequest(response);
            return Ok(response);
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<LogingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<LogingResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (!response.Succeeded) return Unauthorized(response);
            return Ok(response);
        }

        [HttpGet("current-user")]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult> CurrentUser()
        {
            var userId = LoggedInUserId();
            var response = await _authService.GetUserDetailsAsync(userId);
            if (!response.Succeeded) return Unauthorized(response);
            return Ok(response);
        }
    }
}
