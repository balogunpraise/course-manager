namespace Core.Domain.Entities
{
    public class Waitlist : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CourseSectionId { get; set; }
        public int Position { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public bool IsNotified { get; set; }

        // Navigation
        public Student Student { get; set; } = null!;
    }
}
