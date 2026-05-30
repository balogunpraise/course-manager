using Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Application.Dtos.Responses
{
    public class GetCourseResponse
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public int CreditHours { get; set; }
        public Guid DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        // Navigation
        public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
        public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();
    }
}
