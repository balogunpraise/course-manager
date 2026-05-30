using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Transcript : BaseEntity
    {
        public Guid StudentId { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public decimal CumulativeGPA { get; set; }
        public int TotalCreditsEarned { get; set; }
        public string AcademicStanding { get; set; } = string.Empty; // Good, Probation, Honours

        // Navigation
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
    }
}
