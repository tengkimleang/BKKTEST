using Microsoft.Extensions.DependencyInjection;
using Refit;
using Tri_Wall.Shared.Pages.GoodReceptPo;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Shared;

namespace Tri_Wall.Shared.ViewModels;

public static class Dependencies
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri("http://localhost:5253");
            })
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
        services.AddSingleton<ApiService>();
        services.AddScoped<GoodReceptPoViewModel>();
        services.AddSingleton<ILoadMasterData, LoadMasterData>();
        services.AddHostedService<LoadMasterDataService>();
        return services;
    }
}
