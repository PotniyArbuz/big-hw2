using FileStoringService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<FileStoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FileDb")));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storing Service", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileStoringDbContext>();
    bool dbReady = false;
    for (int i = 0; i < 20; i++)
    {
        try
        {
            dbContext.Database.Migrate();
            dbReady = true;
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration attempt {i + 1} failed: {ex.Message}");
            Thread.Sleep(3000);
        }
    }

    if (!dbReady)
    {
        Console.WriteLine("Failed to connect to postgres-file-storing after 20 attempts.");
        throw new Exception("Database initialization failed.");
    }
}

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storing Service v1"));
app.MapControllers();

app.Run();