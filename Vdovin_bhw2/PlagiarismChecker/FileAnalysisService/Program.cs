using FileAnalysisService.Data;
using FileAnalysisService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<FileAnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AnalysisDb")));
builder.Services.AddHttpClient("FileStoringClient", client =>
{
    client.BaseAddress = new Uri("http://file-storing-service:8081");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

builder.Services.AddTransient<FileStoringClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("FileStoringClient");
    return new FileStoringClient(httpClient);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Analysis Service", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FileAnalysisDbContext>();
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient(nameof(FileStoringClient));



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
        Console.WriteLine("Failed to connect to postgres-file-analysis after 20 attempts.");
        throw new Exception("Database initialization failed.");
    }

    bool serviceReady = false;
    for (int i = 0; i < 20; i++)
    {
        try
        {
            var response = await client.GetAsync("/swagger/index.html");
            if (response.IsSuccessStatusCode)
            {
                serviceReady = true;
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FileStoringService attempt {i + 1} failed: {ex.Message}");
            Thread.Sleep(3000);
        }
    }

    if (!serviceReady)
    {
        Console.WriteLine("Failed to connect to file-storing-service after 20 attempts.");
        throw new Exception("FileStoringService initialization failed.");
    }
}

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1"));
app.MapControllers();

app.Run();