using Asp.Versioning;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Caching;
using Server.Application.Abstractions.Clock;
using Server.Application.Abstractions.Data;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Products;
using Server.Domain.Users;
using Server.Infrastructure.Authentication;
using Server.Infrastructure.Authorization;
using Server.Infrastructure.Caching;
using Server.Infrastructure.Clock;
using Server.Infrastructure.Data;
using Server.Infrastructure.Repositories;
using AuthenticationOptions = Server.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Server.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Server.Application.Abstractions.Authentication.IAuthenticationService;

namespace Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        AddPersistence(
            services,
            configuration
        );

        AddCaching(
            services,
            configuration
        );

        AddAuthentication(
            services,
            configuration
        );

        AddAuthorization(
            services
        );

        AddHealthChecks(
            services,
            configuration
        );

        AddApiVersioning(
            services
        );

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(
                "Database"
            )
            ?? throw new ArgumentNullException(
                nameof(configuration)
            );

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString
            ).UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>()
        );

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(
                connectionString
            )
        );

        SqlMapper.AddTypeHandler(
            new DateOnlyTypeHandler()
        );
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme
        ).AddJwtBearer();

        services.Configure<AuthenticationOptions>(
            configuration.GetSection(
                "Authentication"
            )
        );

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(
            configuration.GetSection(
                "Keycloak"
            )
        );

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(
                    keycloakOptions.AdminUrl
                );
            }
        ).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
            {
                KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(
                    keycloakOptions.TokenUrl
                );
            }
        );

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(
                "Cache"
            )
            ?? throw new ArgumentNullException(
                nameof(configuration)
            );

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString
        );

        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddNpgSql(
            configuration.GetConnectionString(
                "Database"
            )!
        ).AddRedis(
            configuration.GetConnectionString(
                "Cache"
            )!
        ).AddUrlGroup(
            new Uri(
                configuration["KeyCloak:BaseUrl"]!
            ),
            HttpMethod.Get,
            "keycloak"
        );
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(
                    1
                );
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }
        ).AddMvc().AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            }
        );
    }
}
