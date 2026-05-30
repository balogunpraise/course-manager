using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Grade : BaseEntity
    {
        public Guid EnrollmentId { get; set; }
        public decimal Points { get; set; }       
        public string LetterGrade { get; set; } = string.Empty; 
        public DateTime GradedAt { get; set; }
        public string Remarks { get; set; }
        [ForeignKey(nameof(EnrollmentId))]
        public Enrollment Enrollment { get; set; } = null!;
    }
}
