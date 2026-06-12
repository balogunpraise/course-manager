using Core.Domain.Entities.LinkingEntities;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Dtos.Responses
{
    public class LecturerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
        public string Email { get; set; }
        public Qualification Qualification { get; set; }
        public Specialization Specialization { get; set; }
        public AcademicRank Rank { get; set; }
        public int YearsOfExperience { get; set; }
        public int MaxCourseLoad { get; set; }
        public bool IsAvailable { get; set; }
        public int AllocatedCoursesCount { get; set; }
        public List<string> PreferredLevels { get; set; }
        public List<string> PreferredCourses { get; set; }
    }

    public class UpdateLecturerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Qualification Qualification { get; set; }
        public Specialization Specialization { get; set; }
        public AcademicRank Rank { get; set; }
        public int YearsOfExperience { get; set; }
        public int MaxCourseLoad { get; set; }
        public List<PreferedLevel> PrefferedLevels { get; set; }
        public List<PreferedCourse> PreferedCourses { get; set; }
    }
}
