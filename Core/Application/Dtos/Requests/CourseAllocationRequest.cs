using System.ComponentModel.DataAnnotations;

namespace Core.Application.Dtos.Requests
{
    public class ManualCourseAllocationRequest
    {
        [Required]
        public Guid LecturerId { get; set; }
        public List<ClassCourseAllocation> Allocations { get; set;}
    }

    public class ClassCourseAllocation
    {
        public Guid LevelId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid CourseId { get; set; }
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
