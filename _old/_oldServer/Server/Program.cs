using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Server.Entities;
using System.Security.Claims;
using Server.Constants;
using Server.DTO.Orders;
using Server.DTO.Shared;
using Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// In security purposes we always want to remove server header from response.
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.Services
    // Custom caching profiles (Response cache).
    .AddControllers(options =>
    {
        options.CacheProfiles.Add("NoCache", new CacheProfile() { NoStore = true });
        options.CacheProfiles.Add(
            "Any-60",
            new CacheProfile() { Location = ResponseCacheLocation.Any, Duration = 60 }
        );
        options.CacheProfiles.Add(
            "Client-120",
            new CacheProfile() { Location = ResponseCacheLocation.Client, Duration = 120 }
        );
    })
    // We need to use NewtonsoftJson in order to resolve correct json serealization.
    // Especaially if we are using enums or nested objects.
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json
            .ReferenceLoopHandling
            .Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // For displaying annotations.
    options.EnableAnnotations();

    // JWT Auth
    // For displaying the auth token in swagger.
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        }
    );
});

// JWT Auth
builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 12;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// JWT Auth
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
            options.DefaultScheme =
            options.DefaultSignInScheme =
            options.DefaultSignOutScheme =
                JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
            )
        };
    });

// JWT Auth custom policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "ModeratorWithMobilePhone",
        policy =>
            policy
                .RequireClaim(ClaimTypes.Role, RoleNames.Moderator)
                .RequireClaim(ClaimTypes.MobilePhone)
    );

    options.AddPolicy(
        "MinAge18",
        policy =>
            policy.RequireAssertion(
                ctx =>
                    ctx.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth)
                    && DateTime.ParseExact(
                        "yyyyMMdd",
                        ctx.User.Claims.First(c => c.Type == ClaimTypes.DateOfBirth).Value,
                        System.Globalization.CultureInfo.InvariantCulture
                    ) >= DateTime.Now.AddYears(-18)
            )
    );
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddMemoryCache();

// For lowercased routes.
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// No Sniffing, No Framing.
app.RemoveInsecureHeaders();

// JWT Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
