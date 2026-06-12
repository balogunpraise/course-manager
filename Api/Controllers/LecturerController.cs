using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Requests.Shared;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public class LecturerController(ILecturerService lecturerService, ICourseAllocationService allocationService) : BaseApiController
    {
        private readonly ILecturerService _lecturerService = lecturerService;
        private readonly ICourseAllocationService _allocationService = allocationService;


        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLecturer([FromBody] CreateLecturerRequest request)
        {
            var response = await _lecturerService.CreateLecturerAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Gets a single lecturer by ID</summary>
        [HttpGet("{lecturerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<LecturerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLecturerById([FromRoute] Guid lecturerId)
        {
            var response = await _lecturerService.GetLecturerByIdAsync(lecturerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Gets all lecturers — paginated, searchable, filterable, sortable</summary>
        /// <remarks>
        /// Filter examples: Specialization:Science, Qualification:Master, IsAvailable:true, Rank:Professor
        /// Sort values: FirstName, LastName, Rank, YearsOfExperience, CreatedAt
        /// Sort order: ASC | DESC
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<PagedList<LecturerResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLecturers([FromQuery] RequestParams request)
        {
            var response = await _lecturerService.GetAllLecturersAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Updates a lecturer's profile</summary>
        [HttpPut("{lecturerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLecturer([FromRoute] Guid lecturerId, [FromBody] UpdateLecturerRequest request)
        {
            var response = await _lecturerService.UpdateLecturerAsync(lecturerId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Toggles a lecturer's availability status</summary>
        [HttpPatch("{lecturerId:guid}/toggle-availability")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleAvailability([FromRoute] Guid lecturerId)
        {
            var response = await _lecturerService.ToggleLecturerAvailabilityAsync(lecturerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Soft deletes a lecturer and their user account</summary>
        [HttpDelete("{lecturerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLecturer([FromRoute] Guid lecturerId)
        {
            var response = await _lecturerService.DeleteLecturerAsync(lecturerId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
