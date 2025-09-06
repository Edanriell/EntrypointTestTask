using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Server.Api.Extensions;
using Server.Api.OpenApi;
using Server.Application;
using Server.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(
    args
);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(
        context.Configuration
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration
);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

WebApplication app = builder.Build();

app.UseCors("AllowedOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            foreach (ApiVersionDescription description in app.DescribeApiVersions())
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(
                    url,
                    name
                );
            }
        }
    );

    app.ApplyMigrations();

    // Uncomment if you want to seed initial data.
    // app.SeedCustomersDataAsync();
    // app.SeedProductsDataAsync();
    // app.SeedOrdersDataAsync();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks(
    "health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

await app.RunAsync();

public partial class Program;

// IMPORTANT
// From Server api root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Server.Infrastructure\Server.Infrastructure.csproj --startup-project .\Server.Api\Server.Api.csproj
