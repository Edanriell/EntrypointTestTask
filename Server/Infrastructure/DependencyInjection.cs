using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitializer>();

        // We can't use simultaneously IdentityConstants.BearerScheme and IdentityConstants.ApplicationScheme (Cookies)
        // We must choose which one we will handle
        // However there is workaround https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET
        // Great article by Rick Strahl, however if we use Identity and it's defaults, a lot of refactor is required
        // Many routes will be gone, so it is overall not worth it
        // Anyway both auths works cookie and jwt, we just can't use them at the same time
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.BearerScheme;
                options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
                options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
                // custom scheme defined in .AddPolicyScheme() below
                // options.DefaultScheme = "JWT_OR_COOKIE";
                // options.DefaultChallengeScheme = "JWT_OR_COOKIE";
            })
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);
        // .AddCookie(IdentityConstants.ApplicationScheme, options =>
        // {
        //     options.LoginPath = "/api/Users/login";
        //     options.ExpireTimeSpan = TimeSpan.FromDays(1);
        // })
        // .AddJwtBearer(IdentityConstants.BearerScheme, options =>
        // {
        //     options.TokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidateIssuer = true,
        //         ValidIssuer = "https://localhost:5001/",
        //         ValidateAudience = true,
        //         ValidAudience = "https://localhost:5001/",
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey =
        //             new SymmetricSecurityKey(Encoding.UTF8.GetBytes("uniqueString"))
        //     };
        // })
        // .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        // {
        //     // runs on each request
        //     options.ForwardDefaultSelector = context =>
        //     {
        //         // filter by auth type
        //         string authorization = context.Request.Headers[HeaderNames.Authorization];
        //         if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        //             return "Bearer";
        //
        //         // otherwise always check for cookie auth
        //         return "Cookies";
        //     };
        // });

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        // Adding policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanManageUserAccount,
                policy =>
                {
                    policy.RequireRole(Roles.Customer, Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator);
                });

            options.AddPolicy(Policies.CanManageUsers,
                policy => { policy.RequireRole(Roles.Administrator, Roles.Moderator, Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanChangeUserRole,
                policy => { policy.RequireRole(Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanManageOrders,
                policy => { policy.RequireRole(Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanDeleteOrders,
                policy => { policy.RequireRole(Roles.Administrator, Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanGetOrder,
                policy =>
                {
                    policy.RequireRole(Roles.Customer, Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator);
                });

            options.AddPolicy(Policies.CanCreateOrder,
                policy =>
                {
                    policy.RequireRole(Roles.Customer, Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator);
                });

            options.AddPolicy(Policies.CanManageProducts,
                policy => { policy.RequireRole(Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanDeleteProducts,
                policy => { policy.RequireRole(Roles.Administrator, Roles.SuperAdministrator); });

            options.AddPolicy(Policies.CanAccessAdminPanel,
                policy => { policy.RequireRole(Roles.Moderator, Roles.Administrator, Roles.SuperAdministrator); });
        });

        return services;
    }
}