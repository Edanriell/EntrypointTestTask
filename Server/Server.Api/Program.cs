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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration
);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

WebApplication app = builder.Build();

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

    // REMARK: Uncomment if you want to seed initial data.
    // app.SeedData();
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

app.Run();

public partial class Program;

// IMPORTANT
// From Server api root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Server.Infrastructure\Server.Infrastructure.csproj --startup-project .\Server.Api\Server.Api.csproj
