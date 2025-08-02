using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Server.Application.Abstractions.Behaviors;
using Server.Domain.OrderProducts;
using Server.Domain.Payments;
using Server.Domain.Products;
using Server.Domain.Users;

namespace Server.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly
                );

                configuration.AddOpenBehavior(
                    typeof(LoggingBehavior<,>)
                );

                configuration.AddOpenBehavior(
                    typeof(ValidationBehavior<,>)
                );

                configuration.AddOpenBehavior(
                    typeof(QueryCachingBehavior<,>)
                );
            }
        );

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );

        // Comes from Domain
        services.AddTransient<OrderPaymentService>();
        services.AddTransient<OrderProductService>();
        services.AddTransient<UserService>();
        services.AddTransient<PaymentService>();
        services.AddTransient<ProductService>();

        return services;
    }
}
