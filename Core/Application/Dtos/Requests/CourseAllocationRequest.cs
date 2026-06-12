namespace Core.Application.Dtos.Requests
{
    public class ManualCourseAllocationRequest
    {
        public Guid LecturerId { get; set; }
        public List<Guid> CourseIds { get; set; }
    }

    public class ReassignCourseRequest
    {
        public Guid FromLecturerId { get; set; }
        public Guid ToLecturerId { get; set; }
        public Guid CourseId { get; set; }
    }

    public class RemoveCourseAllocationRequest
    {
        public Guid LecturerId { get; set; }
        public Guid CourseId { get; set; }
    }

    public class RemoveCoursesBulkRequest
    {
        public Guid LecturerId { get; set; }
        public List<Guid> CourseIds { get; set; }
    }

}
