using Microsoft.Extensions.DependencyInjection;

namespace Tri_Wall.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(option => option.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));
        return services;
    }
}
