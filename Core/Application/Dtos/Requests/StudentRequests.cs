using Core.Application.Dtos.Requests.Shared;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Dtos.Requests
{
    public class CreateStudentRequest
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public Guid DepartmentId { get; set; }
        public Guid LevelId { get; set; }
    }

    public class ListStudentRequest : RequestParams
    {

    }

    public class EnrollStudentRequest
    {
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public List<Guid> CourseIds { get; set; }
    }
}
