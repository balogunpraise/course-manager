using Api.ProjectExtensions;
using Core.Application;
using Core.Domain.Entities.Auth;
using Infrastructure.data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.RegisterServices();
builder.Services.RegisterDatabase(builder.Configuration);
builder.Services.RegisterIdentity();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfile).Assembly);
});
builder.Services.AddCors(option =>
{
    option.AddPolicy("coursemanager", opt =>
    {
        opt.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.RegisterAuth(builder.Configuration);
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Course Manager API",
            Version = "v1",
            Description = "Course Manager API documentation"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();  

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter 'Bearer' followed by your token. Example: 'Bearer eyJhb...'",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        document.Security?.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", document),  
                new List<string>()
            }
        });

        return Task.CompletedTask;
    });
});



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        await new DatabaseSeeder().SeedDatabase(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        throw; // rethrow so the app doesn't start with a broken DB state
    }
}
    

app.MapOpenApi();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "Course Manager API v1");  
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("coursemanager");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
