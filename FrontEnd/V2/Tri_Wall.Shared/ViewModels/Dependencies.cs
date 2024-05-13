using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public static class Dependencies
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(static client => client.BaseAddress = new Uri("http://localhost:5253"))
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
        services.AddSingleton<ApiService>();
        services.TryAddScoped<GoodReceptPoViewModel>();
        return services;
    }
}
