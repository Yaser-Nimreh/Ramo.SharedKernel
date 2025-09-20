using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Behaviors;
using System.Reflection;

namespace SharedKernel.Extensions;

public static class MediatRExtentions
{
    public static IServiceCollection AddMediatRWithAssemblies
        (this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(assemblies);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }
}