using InterviewTest.Server.Services;
using InterviewTest.Server.Middleware;
using InterviewTest.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=SqliteDB.db"));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InterviewTest API",
        Version = "v1",
        Description = "Employees CRUD + bulk update endpoints"
    });

    // Include XML comments if the file exists (we'll enable generation later in the .csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Dev-only CORS to allow the Vite client during development
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientDev", policy =>
        policy.WithOrigins("https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DatabaseSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InterviewTest API v1");
    c.RoutePrefix = "swagger"; // UI at /swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("ClientDev");

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapFallbackToFile("/index.html");

app.Run();
