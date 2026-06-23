namespace Core.Domain.Entities.LinkingEntities
{
    public class LecturerCourse : BaseEntity
    {
        public Guid LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid LevelId { get; set; }
        public Level Level { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
