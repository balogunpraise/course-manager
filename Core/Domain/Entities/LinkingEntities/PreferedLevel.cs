namespace Core.Domain.Entities.LinkingEntities
{
    public class PreferedLevel : BaseEntity
    {
        public Guid LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }
        public Guid LevelId { get; set; }
        public Level Level { get; set; }
    }
}
