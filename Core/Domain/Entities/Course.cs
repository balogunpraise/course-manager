using Core.Domain.Entities.LinkingEntities;
using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string CourseCode { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public bool IsGeneralStudies { get; set; }
        public Guid? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty Faculty { get; set; }
        public Specialization RequiredSpecialization { get; set; }
        public Qualification RequiredQualification { get; set; }
        public bool IsElective { get; set; }
        public Guid? DepartmentId { get; set; }
        public Level Level { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<LecturerCourse> LecturerCourses { get; set; } = new List<LecturerCourse>();
    }
}
