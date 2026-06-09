using Core.Domain.Entities.LinkingEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class CourseActivity : BaseEntity
    {
        // Points to the join table, not directly to Course
        public Guid StudentCourseId { get; set; }
        [ForeignKey(nameof(StudentCourseId))]
        public StudentCourse StudentCourse { get; set; }

        public bool Completed { get; set; }
    }
}
