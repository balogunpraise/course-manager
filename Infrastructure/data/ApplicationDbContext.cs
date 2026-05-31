using Core.Domain.Entities;
using Core.Domain.Entities.Auth;
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

            builder.Entity<Enrollment>()
                .HasIndex(x => x.StudentId);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Prerequisite> Prerequisites { get; set; }
        public DbSet<Transcript> Transcripts { get; set; }
        public DbSet<Waitlist> Waitlists { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

    }
}
