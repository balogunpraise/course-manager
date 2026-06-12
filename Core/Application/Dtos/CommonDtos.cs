using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Dtos
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public Guid LecturerId { get; set; }
        public string LecturerName { get; set; }
        public Guid ClassroomId { get; set; }
        public string ClassroomName { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public ScheduleStatus Status { get; set; }
        public string AcademicSession { get; set; }
        public int Semester { get; set; }
    }

    //public class CreateScheduleDto
    //{
    //    [Required]
    //    public Guid CourseId { get; set; }

    //    [Required]
    //    public Guid LecturerId { get; set; }

    //    [Required]
    //    public Guid ClassroomId { get; set; }

    //    [Required]
    //    public DayOfWeek Day { get; set; }

    //    [Required]
    //    public TimeOnly StartTime { get; set; }

    //    [Required]
    //    public TimeOnly EndTime { get; set; }

    //    [Required]
    //    [Range(1, 2)]
    //    public int Semester { get; set; }

    //    [Required]
    //    [RegularExpression(@"^\d{4}/\d{4}$", ErrorMessage = "Academic session must be in format YYYY/YYYY")]
    //    public string AcademicSession { get; set; }
    //}

    //public class UpdateScheduleDto : CreateScheduleDto
    //{
    //    public ScheduleStatus Status { get; set; }
    //}

    public class CheckScheduleDto
    {
        public Guid LecturerId { get; set; }
        public Guid ClassroomId { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public class ClassroomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public int Capacity { get; set; }
        public ClassroomType Type { get; set; }
        public bool HasProjector { get; set; }
        public bool IsAvailable { get; set; }
        public int ScheduleCount { get; set; }
    }

    public class CreateClassroomDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Building { get; set; }

        [Required]
        [Range(1, 500)]
        public int Capacity { get; set; }

        public ClassroomType Type { get; set; } = ClassroomType.LectureHall;
        public bool HasProjector { get; set; } = false;
        public bool HasWhiteboard { get; set; } = true;
        public bool IsAccessible { get; set; } = true;
    }

    public class UpdateClassroomDto : CreateClassroomDto
    {
        public bool IsAvailable { get; set; }
    }

    public class LecturerAvailabilityDto
    {
        public Guid Id { get; set; }
        public Guid LecturerId { get; set; }
        public string LecturerName { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateLecturerAvailabilityDto
    {
        [Required]
        public Guid LecturerId { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateLecturerAvailabilityDto
    {
        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class CreateBatchAvailabilityDto
    {
        [Required]
        public Guid LecturerId { get; set; }

        [Required]
        public List<DayOfWeek> Days { get; set; }

        [Required]
        public List<TimeSlotDto> TimeSlots { get; set; }
    }

    public class TimeSlotDto
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

}
