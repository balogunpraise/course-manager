using Core.Domain.Entities;
using Core.Domain.Entities.Auth;
using Core.Domain.Entities.LinkingEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

            builder.Entity<CourseActivity>(entity =>
            {
                entity.HasOne(ca => ca.StudentCourse)
                    .WithMany(sc => sc.CourseActivities)
                    .HasForeignKey(ca => ca.StudentCourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

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

    }
}
