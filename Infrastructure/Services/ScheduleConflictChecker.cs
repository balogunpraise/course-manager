using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.data;

namespace Infrastructure.Services
{
    public class ScheduleConflictChecker
    {
        private readonly ApplicationDbContext _context;

        public ScheduleConflictChecker(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Check if lecturer is already booked at this time
        public bool LecturerHasConflict(Guid lecturerId, DayOfWeek day,
                                  TimeOnly start, TimeOnly end, Guid? excludeScheduleId = null)
        {
            var query = _context.Schedules
                .Where(s => s.LecturerId == lecturerId &&
                            s.Day == day &&
                            s.Status == ScheduleStatus.Approved &&
                            !s.IsDeleted) // Assuming BaseEntity has IsDeleted
                .Where(s => !(end <= s.StartTime || start >= s.EndTime)); // Overlap check

            if (excludeScheduleId.HasValue)
            {
                query = query.Where(s => s.Id != excludeScheduleId.Value);
            }

            return query.Any();
        }

        // 2. Check if classroom is already booked at this time
        public bool ClassroomHasConflict(Guid classroomId, DayOfWeek day,
                                   TimeOnly start, TimeOnly end, Guid? excludeScheduleId = null)
        {
            var query = _context.Schedules
                .Where(s => s.ClassroomId == classroomId &&
                            s.Day == day &&
                            s.Status == ScheduleStatus.Approved &&
                            !s.IsDeleted)
                .Where(s => !(end <= s.StartTime || start >= s.EndTime)); // Overlap check

            if (excludeScheduleId.HasValue)
            {
                query = query.Where(s => s.Id != excludeScheduleId.Value);
            }

            return query.Any();
        }

        // 3. Check if lecturer is marked available at this time
        public bool LecturerIsAvailable(Guid lecturerId, DayOfWeek day,
                                  TimeOnly start, TimeOnly end)
        {
            // First, check if lecturer has any availability records for this day
            var availabilities = _context.LecturerAvailabilities
                .Where(a => a.LecturerId == lecturerId &&
                            a.Day == day &&
                            !a.IsDeleted)
                .ToList();

            // If no availability records exist, assume lecturer is NOT available
            if (!availabilities.Any())
                return false;

            // Check if the requested time falls within any availability window
            foreach (var availability in availabilities)
            {
                // Check if requested time is within availability window
                if (start >= availability.StartTime && end <= availability.EndTime)
                {
                    // Also check if the lecturer is marked as available during this time
                    return availability.IsAvailable;
                }
            }

            return false;
        }

        // Additional helper method to check all conflicts at once
        public ScheduleConflictResult CheckAllConflicts(Schedule schedule)
        {
            var result = new ScheduleConflictResult();

            // Check lecturer conflicts
            result.HasLecturerConflict = LecturerHasConflict(
                schedule.LecturerId,
                schedule.Day,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Id
            );

            if (result.HasLecturerConflict)
                result.Conflicts.Add($"Lecturer {schedule.Lecturer?.FirstName} {schedule.Lecturer?.LastName} is already booked at this time.");

            // Check classroom conflicts
            result.HasClassroomConflict = ClassroomHasConflict(
                schedule.ClassroomId,
                schedule.Day,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Id
            );

            if (result.HasClassroomConflict)
                result.Conflicts.Add($"Classroom {schedule.Classroom?.Name} is already booked at this time.");

            // Check lecturer availability
            result.HasAvailabilityIssue = !LecturerIsAvailable(
                schedule.LecturerId,
                schedule.Day,
                schedule.StartTime,
                schedule.EndTime
            );

            if (result.HasAvailabilityIssue)
                result.Conflicts.Add($"Lecturer is not available at this time according to their schedule.");

            result.IsValid = !result.HasLecturerConflict &&
                            !result.HasClassroomConflict &&
                            !result.HasAvailabilityIssue;

            return result;
        }

        // Check course load for lecturer
        public bool LecturerHasCourseLoadCapacity(Guid lecturerId, Semester semester, string academicSession)
        {
            var lecturer = _context.Lecturers.Find(lecturerId);
            if (lecturer == null) return false;

            var currentCourses = _context.Schedules
                .Where(s => s.LecturerId == lecturerId &&
                            s.Semester == semester &&
                            s.AcademicSession == academicSession &&
                            s.Status == ScheduleStatus.Approved &&
                            !s.IsDeleted)
                .Select(s => s.Course)
                .Distinct()
                .Count();

            return currentCourses < lecturer.MaxCourseLoad;
        }

        // Get available classrooms for a given time slot
        public List<Classroom> GetAvailableClassrooms(DayOfWeek day, TimeOnly start, TimeOnly end, int? minCapacity = null)
        {
            var bookedClassrooms = _context.Schedules
                .Where(s => s.Day == day &&
                            s.Status == ScheduleStatus.Approved &&
                            !s.IsDeleted &&
                            !(end <= s.StartTime || start >= s.EndTime))
                .Select(s => s.ClassroomId)
                .Distinct();

            var query = _context.Classrooms
                .Where(c => !bookedClassrooms.Contains(c.Id) && c.IsAvailable);

            if (minCapacity.HasValue)
            {
                query = query.Where(c => c.Capacity >= minCapacity.Value);
            }

            return query.ToList();
        }

        // Get available time slots for a lecturer on a specific day
        public List<TimeSlot> GetAvailableTimeSlotsForLecturer(Guid lecturerId, DayOfWeek day, int durationMinutes = 60)
        {
            var availableSlots = new List<TimeSlot>();

            // Get lecturer's availability for the day
            var availabilities = _context.LecturerAvailabilities
                .Where(a => a.LecturerId == lecturerId && a.Day == day && a.IsAvailable)
                .ToList();

            if (!availabilities.Any())
                return availableSlots;

            // Get existing bookings for the lecturer
            var bookings = _context.Schedules
                .Where(s => s.LecturerId == lecturerId &&
                            s.Day == day &&
                            s.Status == ScheduleStatus.Approved &&
                            !s.IsDeleted)
                .Select(s => new { s.StartTime, s.EndTime })
                .ToList();

            foreach (var availability in availabilities)
            {
                var currentStart = availability.StartTime;

                while (currentStart.AddMinutes(durationMinutes) <= availability.EndTime)
                {
                    var potentialEnd = currentStart.AddMinutes(durationMinutes);

                    // Check if this time slot conflicts with any booking
                    bool hasConflict = bookings.Any(b =>
                        !(potentialEnd <= b.StartTime || currentStart >= b.EndTime));

                    if (!hasConflict)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            StartTime = currentStart,
                            EndTime = potentialEnd,
                            IsAvailable = true
                        });
                    }

                    currentStart = currentStart.AddMinutes(30); // Increment by 30 minutes
                }
            }

            return availableSlots;
        }
    }

    // Supporting classes
    public class ScheduleConflictResult
    {
        public bool IsValid { get; set; }
        public bool HasLecturerConflict { get; set; }
        public bool HasClassroomConflict { get; set; }
        public bool HasAvailabilityIssue { get; set; }
        public List<string> Conflicts { get; set; } = new List<string>();
    }

    public class TimeSlot
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
