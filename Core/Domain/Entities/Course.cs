using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string CourseCode { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        // Navigation
        public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
        public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();
    }
}
