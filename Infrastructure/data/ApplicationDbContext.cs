using Core.Domain.Entities;
using Core.Domain.Entities.Auth;
using Core.Domain.Entities.LinkingEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure.data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => sc.Id);
                entity.HasIndex(sc => new { sc.StudentId, sc.CourseId }).IsUnique();
                entity.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(sc => sc.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(sc => sc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);


            });

            builder.Entity<LecturerCourse>(entity =>
            {
                entity.HasKey(lc => lc.Id);
                entity.HasIndex(lc => new { lc.LecturerId, lc.CourseId }).IsUnique();


                entity.HasOne(lc => lc.Lecturer)
                    .WithMany(lc => lc.LecturerCourses)
                    .HasForeignKey(lc => lc.LecturerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lc => lc.Course)
                    .WithMany(lc => lc.LecturerCourses)
                    .HasForeignKey(lc => lc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);


            });

            builder.Entity<CourseActivity>(entity =>
            {
                entity.HasOne(ca => ca.StudentCourse)
                    .WithMany(sc => sc.CourseActivities)
                    .HasForeignKey(ca => ca.StudentCourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Schedule>()
                .HasIndex(s => new { s.LecturerId, s.Day, s.StartTime, s.Semester })
                .HasDatabaseName("IX_Schedule_Lecturer_Time");

            builder.Entity<Schedule>()
                .HasIndex(s => new { s.ClassroomId, s.Day, s.StartTime, s.Semester })
                .HasDatabaseName("IX_Schedule_Classroom_Time");

        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Transcript> Transcripts { get; set; }
        public DbSet<Waitlist> Waitlists { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<LecturerCourse> LecturerCourses { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<PreferedLevel> PreferredLevels { get; set; }
        public DbSet<PreferedCourse> PreferedCourses { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<LecturerAvailability> LecturerAvailabilities { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

    }
}
