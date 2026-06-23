using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities.Auth;
using Infrastructure.data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.ProjectExtensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>()
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<ICourseAllocationService, CourseAllocationService>();
            services.AddScoped<ScheduleConflictChecker>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ISemesterService, SemesterService>();
            return services;
        }

        public static IServiceCollection RegisterAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };
            });
            services.AddAuthorization();
            return services;
        }
    }
}
