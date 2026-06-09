using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid FacultyId { get; set; }
        [ForeignKey(nameof(FacultyId))]
        public Faculty Faculty { get; set; }
    }
}
