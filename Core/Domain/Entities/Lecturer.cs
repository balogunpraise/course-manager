using Core.Domain.Entities.Auth;
using Core.Domain.Entities.LinkingEntities;
using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Lecturer : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Qualification Qualification { get; set; }
        public Specialization Specialization { get; set; }
        public AcademicRank Rank { get; set; }
        public int YearsOfExperience { get; set; }
        public int MaxCourseLoad { get; set; }
        public bool IsAvailable { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        public List<PreferedLevel> PrefferedLevels { get; set; } = new List<PreferedLevel>();
        public List<LecturerCourse> LecturerCourses { get; set; } = new List<LecturerCourse>();
        public List<PreferedCourse> PreferedCourses { get; set; } = new List<PreferedCourse>();

    }
}
