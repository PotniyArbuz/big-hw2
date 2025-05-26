using ApiGateway.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<FileStoringClient>(client =>
{
    client.BaseAddress = new Uri("http://file-storing-service:8081");
});
builder.Services.AddHttpClient<FileAnalysisClient>(client =>
{
    client.BaseAddress = new Uri("http://file-analysis-service:8082");
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
});

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
app.MapControllers();

app.Run();