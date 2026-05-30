using Core.Application.Dtos.Requests.Shared;

namespace Core.Application.Dtos.Requests
{
    public class CreateCourseRequest
    {
        public string CourseCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        //public Guid? DepartmentId { get; set; }
        // Navigation
    }


    public class UpdateCourseRequest
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditHours { get; set; }
    }

    public class ListCoursesRequest : RequestParams
    {
    }
}
