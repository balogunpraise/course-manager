using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class LecturerAvailability : BaseEntity
    {
        public Guid LecturerId { get; set; }
        [ForeignKey(nameof(LecturerId))]
        public Lecturer Lecturer { get; set; }

        public DayOfWeek Day { get; set; }         // Monday, Tuesday...
        public TimeOnly StartTime { get; set; }    // e.g. 08:00
        public TimeOnly EndTime { get; set; }      // e.g. 12:00
        public bool IsAvailable { get; set; }      // false = blocked (e.g. on leave)
    }
}
