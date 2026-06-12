using Core.Domain.Entities.LinkingEntities;
using Core.Domain.Enums;

namespace Core.Application.Dtos.Requests
{
    public class CreateLecturerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Qualification Qualification { get; set; }
        public Specialization Specialization { get; set; }
        public AcademicRank Rank { get; set; }
        public int YearsOfExperience { get; set; }
        public List<PreferedLevelDto> PrefferedLevels { get; set; } = new List<PreferedLevelDto>();
        public List<PreferedCourseDto> PreferedCourses { get; set; } = new List<PreferedCourseDto>();
    }


    public class PreferedLevelDto
    {
        public Guid LevelId { get; set; }   // LecturerId removed
    }

    public class PreferedCourseDto
    {
        public Guid CourseId { get; set; }  // LecturerId removed
    }
}
