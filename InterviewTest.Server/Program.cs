using InterviewTest.Server.Middleware;
using InterviewTest.Server.Data;
using InterviewTest.Server.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=SqliteDB.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (app.Environment.IsDevelopment())
    {
        DatabaseSeeder.Seed(db);
    }
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
