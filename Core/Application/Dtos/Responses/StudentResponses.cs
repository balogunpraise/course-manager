using Core.Domain.Entities;
using Core.Domain.Entities.Auth;
using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Application.Dtos.Responses
{
    public class ListStudentResponses
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; } 
        public string StudentNumber { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } 
        public decimal GPA { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid LevelId { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
    }


    public class GetStudentDetailsResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; } 
        public string StudentNumber { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } 
        public decimal GPA { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid LevelId { get; set; }
        public ICollection<Transcript> Transcripts { get; set; } = new List<Transcript>();
        public ICollection<Waitlist> WaitlistEntries { get; set; } = new List<Waitlist>();
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
    }
}
