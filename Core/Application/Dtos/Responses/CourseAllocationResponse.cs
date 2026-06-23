using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Dtos.Responses
{
    public class AllocatedCourseResponse
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }
        public string Level { get; set; }
        public bool IsElective { get; set; }
        public bool IsGeneralStudies { get; set; }
        public Qualification RequiredQualification { get; set; }
        public Specialization RequiredSpecialization { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public DateTime AllocatedOn { get; set; }
    }
}
