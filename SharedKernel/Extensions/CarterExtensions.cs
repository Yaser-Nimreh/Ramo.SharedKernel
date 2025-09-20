using Carter;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SharedKernel.Extensions;

public static class CarterExtensions
{
    public static IServiceCollection AddCarterWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddCarter(configurator: configuration =>
        {
            foreach (var assembly in assemblies)
            {
                var modules = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

                configuration.WithModules(modules);
            }
        });

        return services;
    }
}