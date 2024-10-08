using Application;
using Infrastructure;
using Infrastructure.Data;
using Web;
using Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Without this code you can't do external api calls

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		policy =>
		{
			policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
			   .AllowAnyMethod()
			   .AllowAnyHeader();
		});
});


builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
	await app.InitialiseDatabaseAsync();
else
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
	settings.Path         = "/api";
	settings.DocumentPath = "/api/specification.json";
});

app.MapControllerRoute(
	"default",
	"{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.UseCors();

app.Run();

public partial class Program
{
}