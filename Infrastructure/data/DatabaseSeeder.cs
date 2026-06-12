using Core.Domain.Entities;
using Core.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.data
{
    public class DatabaseSeeder
    {
        public async Task SeedDatabase(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            await ApplyMigrations(context);
            await SeedAdminUser(userManager, roleManager);
            await SeedFaculties(context);
            await SeedDepartments(context);
            await SeedCourses(context);
            await SeedLevels(context);
        }

        private static async Task ApplyMigrations(ApplicationDbContext context)
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations?.Any() == true)
            {
                await context.Database.MigrateAsync();
            }
            
        }

        private static async Task SeedAdminUser(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            await SeedRoles(roleManager);
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    FirstName = "John",
                    LastName = "Admin",
                    PhoneNumber = "09011389876",
                    UserName = "admin",
                    Email = "admin@example.com"
                };
                var result = await userManager.CreateAsync(user, "Password123!");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to seed admin user: {errors}");
                }
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private static async Task SeedRoles(RoleManager<AppRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var roles = new[] { "Admin", "Student" };
                foreach (var role in roles)
                {
                    var result = await roleManager.CreateAsync(new AppRole { Name = role });
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to seed role '{role}': {errors}");
                    }
                }
            }
        }

        private static async Task SeedFaculties(ApplicationDbContext context)
        {
            if (!context.Faculties.Any())
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Faculty.json");
                if (!File.Exists(file)) throw new FileNotFoundException(file);
                var facultiesJson = await File.ReadAllTextAsync(file);
                var faculties = JsonConvert.DeserializeObject<List<Faculty>>(facultiesJson);
                if(faculties != null)
                {
                    await context.Faculties.AddRangeAsync(faculties);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedDepartments(ApplicationDbContext context)
        {
            if (!context.Departments.Any())
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Department.json");
                if (!File.Exists(file)) throw new FileNotFoundException(file);
                var departmentsJson = await File.ReadAllTextAsync(file);
                var departments = JsonConvert.DeserializeObject<List<Department>>(departmentsJson);
                if (departments != null)
                {
                    await context.Departments.AddRangeAsync(departments);
                    await context.SaveChangesAsync();
                }
                    
            }
        }

        public static async Task SeedCourses(ApplicationDbContext context)
        {
            if (!context.Courses.Any())
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Course.json");
                if (!File.Exists(file)) throw new FileNotFoundException(file);
                var coursesJson = await File.ReadAllTextAsync(file);
                var courses = JsonConvert.DeserializeObject<List<Course>>(coursesJson);
                if (courses != null)
                {
                    await context.Courses.AddRangeAsync(courses);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task DeleteCourse(ApplicationDbContext context)
        {
            if(context.Courses.Any())
            {
                var courses = await context.Courses.ToListAsync();
                if (courses != null && courses.Any())
                {
                    context.Courses.RemoveRange(courses);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task SeedLevels(ApplicationDbContext context)
        {
            if (!context.Levels.Any())
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Levels.json");
                if (!File.Exists(file)) throw new FileNotFoundException(file);
                var levelsJson = await File.ReadAllTextAsync(file);
                var levels = JsonConvert.DeserializeObject<List<Level>>(levelsJson);
                if (levels != null)
                {
                    await context.Levels.AddRangeAsync(levels);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
