using Core.Application;
using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities.LinkingEntities;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class CourseAllocationService : ICourseAllocationService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;

        public CourseAllocationService(IOptions<AppSettings> appSettings, ApplicationDbContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public async Task<BaseResponse> AllocateCoursesManually(ManualCourseAllocationRequest request)
        {
            var errors = new List<string>();

            // 1. Validate lecturer (single query with courses already allocated)
            var lecturer = await _context.Lecturers
                .Include(l => l.LecturerCourses)
                .FirstOrDefaultAsync(l => l.Id == request.LecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            if (!lecturer.IsAvailable)
                return BaseResponse.Failure(400, "Lecturer is currently unavailable for course allocation");

            // 2. Fetch all requested courses in one query
            var courses = await _context.Courses
                .Where(c => request.CourseIds.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            // Check all courses exist
            if (courses.Count != request.CourseIds.Count)
            {
                var foundIds = courses.Select(c => c.Id).ToHashSet();
                var missingIds = request.CourseIds.Where(id => !foundIds.Contains(id));
                return BaseResponse.Failure(404, "One or more courses not found",
                    missingIds.Select(id => $"Course {id} not found").ToList());
            }

            // 3. Get already-allocated course IDs for this lecturer (avoid N+1)
            var alreadyAllocatedCourseIds = lecturer.LecturerCourses
                .Select(lc => lc.CourseId)
                .ToHashSet();

            // 4. Filter to only new courses
            var newCourses = courses
                .Where(c => !alreadyAllocatedCourseIds.Contains(c.Id))
                .ToList();

            if (newCourses.Count == 0)
                return BaseResponse.Failure(400, "All selected courses are already allocated to this lecturer");

            // 5. Validate each course against lecturer profile
            foreach (var course in newCourses)
            {
                if (course.RequiredSpecialization != lecturer.Specialization)
                    errors.Add($"[{course.CourseCode}] Specialization mismatch — requires {course.RequiredSpecialization}, lecturer has {lecturer.Specialization}");

                if (course.RequiredQualification > lecturer.Qualification)
                    errors.Add($"[{course.CourseCode}] Qualification too low — requires {course.RequiredQualification}, lecturer has {lecturer.Qualification}");
            }

            if (errors.Any())
                return BaseResponse.Failure(400, "One or more courses failed validation", errors);

            // 6. Workload check
            int currentLoad = lecturer.LecturerCourses.Count;
            int projectedLoad = currentLoad + newCourses.Count;

            if (projectedLoad > lecturer.MaxCourseLoad)
                return BaseResponse.Failure(400,
                    $"Allocation exceeds lecturer's max course load. " +
                    $"Current: {currentLoad}, Requested: {newCourses.Count}, Max: {lecturer.MaxCourseLoad}");

            // 7. Bulk insert new allocations
            var allocations = newCourses.Select(course => new LecturerCourse
            {
                LecturerId = request.LecturerId,
                CourseId = course.Id
            }).ToList();

            await _context.LecturerCourses.AddRangeAsync(allocations);
            await _context.SaveChangesAsync();

            return BaseResponse.Success(200,
                $"{newCourses.Count} course(s) successfully allocated to {lecturer.FirstName} {lecturer.LastName}");
        }


        public async Task<BaseResponse<List<AllocatedCourseResponse>>> GetLecturerAllocatedCourses(Guid lecturerId)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse<List<AllocatedCourseResponse>>.Failure(404, "Lecturer not found");

            var allocatedCourses = await _context.LecturerCourses
                .Where(lc => lc.LecturerId == lecturerId)
                .Include(lc => lc.Course)
                    .ThenInclude(c => c.Department)
                .Include(lc => lc.Course)
                    .ThenInclude(c => c.Faculty)
                .Select(lc => new AllocatedCourseResponse
                {
                    CourseId = lc.Course.Id,
                    CourseCode = lc.Course.CourseCode,
                    Title = lc.Course.Title,
                    Description = lc.Course.Description,
                    CreditHours = lc.Course.CreditHours,
                    Level = lc.Course.Level,
                    IsElective = lc.Course.IsElective,
                    IsGeneralStudies = lc.Course.IsGeneralStudies,
                    RequiredQualification = lc.Course.RequiredQualification,
                    RequiredSpecialization = lc.Course.RequiredSpecialization,
                    Department = lc.Course.Department.Name,
                    Faculty = lc.Course.Faculty.Name,
                    AllocatedOn = lc.CreatedAt
                })
                .ToListAsync();

            if (!allocatedCourses.Any())
                return BaseResponse<List<AllocatedCourseResponse>>.Success(200, "Lecturer has no allocated courses", new List<AllocatedCourseResponse>());

            return BaseResponse<List<AllocatedCourseResponse>>.Success(
                200,
                $"{allocatedCourses.Count} course(s) found",
                allocatedCourses
            );
        }


        public async Task<BaseResponse> ReassignCourse(ReassignCourseRequest request)
        {
            // 1. Validate both lecturers in one query
            var lecturerIds = new[] { request.FromLecturerId, request.ToLecturerId };
            var lecturers = await _context.Lecturers
                .Include(l => l.LecturerCourses)
                .Where(l => lecturerIds.Contains(l.Id) && !l.IsDeleted)
                .ToListAsync();

            var fromLecturer = lecturers.FirstOrDefault(l => l.Id == request.FromLecturerId);
            var toLecturer = lecturers.FirstOrDefault(l => l.Id == request.ToLecturerId);

            if (fromLecturer == null)
                return BaseResponse.Failure(404, "Source lecturer not found");

            if (toLecturer == null)
                return BaseResponse.Failure(404, "Target lecturer not found");

            if (!toLecturer.IsAvailable)
                return BaseResponse.Failure(400, $"{toLecturer.FirstName} {toLecturer.LastName} is currently unavailable");

            if (request.FromLecturerId == request.ToLecturerId)
                return BaseResponse.Failure(400, "Source and target lecturer cannot be the same");

            // 2. Validate course exists
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == request.CourseId && !c.IsDeleted);

            if (course == null)
                return BaseResponse.Failure(404, "Course not found");

            // 3. Confirm course is actually assigned to source lecturer
            var existingAllocation = await _context.LecturerCourses
                .FirstOrDefaultAsync(lc => lc.LecturerId == request.FromLecturerId && lc.CourseId == request.CourseId);

            if (existingAllocation == null)
                return BaseResponse.Failure(400,
                    $"[{course.CourseCode}] is not allocated to {fromLecturer.FirstName} {fromLecturer.LastName}");

            // 4. Check course is not already allocated to target lecturer
            var alreadyAssigned = toLecturer.LecturerCourses.Any(lc => lc.CourseId == request.CourseId);
            if (alreadyAssigned)
                return BaseResponse.Failure(400,
                    $"[{course.CourseCode}] is already allocated to {toLecturer.FirstName} {toLecturer.LastName}");

            // 5. Validate target lecturer's profile against course requirements
            var errors = new List<string>();

            if (course.RequiredSpecialization != toLecturer.Specialization)
                errors.Add($"Specialization mismatch — requires {course.RequiredSpecialization}, lecturer has {toLecturer.Specialization}");

            if (course.RequiredQualification > toLecturer.Qualification)
                errors.Add($"Qualification too low — requires {course.RequiredQualification}, lecturer has {toLecturer.Qualification}");

            if (errors.Any())
                return BaseResponse.Failure(400, "Target lecturer does not meet course requirements", errors);

            // 6. Workload check on target lecturer
            int currentLoad = toLecturer.LecturerCourses.Count;
            if (currentLoad + 1 > toLecturer.MaxCourseLoad)
                return BaseResponse.Failure(400,
                    $"{toLecturer.FirstName} {toLecturer.LastName} has reached their max course load of {toLecturer.MaxCourseLoad}");

            // 7. Reassign — remove from source, add to target
            _context.LecturerCourses.Remove(existingAllocation);
            await _context.LecturerCourses.AddAsync(new LecturerCourse
            {
                LecturerId = request.ToLecturerId,
                CourseId = request.CourseId
            });

            await _context.SaveChangesAsync();

            return BaseResponse.Success(200,
                $"[{course.CourseCode}] {course.Title} successfully reassigned from " +
                $"{fromLecturer.FirstName} {fromLecturer.LastName} to {toLecturer.FirstName} {toLecturer.LastName}");
        }


        public async Task<BaseResponse> RemoveCoursesFromLecturer(RemoveCoursesBulkRequest request)
        {
            // 1. Validate lecturer
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == request.LecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            // 2. Fetch all matching allocations in one query
            var allocations = await _context.LecturerCourses
                .Include(lc => lc.Course)
                .Where(lc => lc.LecturerId == request.LecturerId && request.CourseIds.Contains(lc.CourseId))
                .ToListAsync();

            if (!allocations.Any())
                return BaseResponse.Failure(400, "None of the selected courses are allocated to this lecturer");

            // 3. Report any course IDs that were not found
            var foundCourseIds = allocations.Select(lc => lc.CourseId).ToHashSet();
            var notAllocated = request.CourseIds
                .Where(id => !foundCourseIds.Contains(id))
                .ToList();

            // 4. Bulk remove
            _context.LecturerCourses.RemoveRange(allocations);
            await _context.SaveChangesAsync();

            var removedCourses = allocations.Select(lc => $"[{lc.Course.CourseCode}] {lc.Course.Title}").ToList();

            var message = notAllocated.Any()
                ? $"{allocations.Count} course(s) removed. {notAllocated.Count} course ID(s) were not allocated and skipped."
                : $"{allocations.Count} course(s) successfully removed from {lecturer.FirstName} {lecturer.LastName}";

            return BaseResponse.Success(200, message);
        }

        public async Task<BaseResponse> BulkRemoveCoursesFromLecturer(RemoveCoursesBulkRequest request)
        {
            // 1. Validate lecturer
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == request.LecturerId && !l.IsDeleted);

            if (lecturer == null)
                return BaseResponse.Failure(404, "Lecturer not found");

            // 2. Fetch all matching allocations in one query
            var allocations = await _context.LecturerCourses
                .Include(lc => lc.Course)
                .Where(lc => lc.LecturerId == request.LecturerId && request.CourseIds.Contains(lc.CourseId))
                .ToListAsync();

            if (!allocations.Any())
                return BaseResponse.Failure(400, "None of the selected courses are allocated to this lecturer");

            // 3. Report any course IDs that were not found
            var foundCourseIds = allocations.Select(lc => lc.CourseId).ToHashSet();
            var notAllocated = request.CourseIds
                .Where(id => !foundCourseIds.Contains(id))
                .ToList();

            // 4. Bulk remove
            _context.LecturerCourses.RemoveRange(allocations);
            await _context.SaveChangesAsync();

            var removedCourses = allocations.Select(lc => $"[{lc.Course.CourseCode}] {lc.Course.Title}").ToList();

            var message = notAllocated.Any()
                ? $"{allocations.Count} course(s) removed. {notAllocated.Count} course ID(s) were not allocated and skipped."
                : $"{allocations.Count} course(s) successfully removed from {lecturer.FirstName} {lecturer.LastName}";

            return BaseResponse.Success(200, message);
        }
    }
}
