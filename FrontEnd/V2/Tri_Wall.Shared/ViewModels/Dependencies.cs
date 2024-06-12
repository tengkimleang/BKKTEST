using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public static class Dependencies
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri("http://localhost:8429/BarCodeBackEnd");
            })
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
        services.AddSingleton<ApiService>();
        services.AddScoped<GoodReceptPoViewModel>();
        services.AddScoped<DeliveryOrderViewModel>();
        #region Validator
        services.AddScoped<IValidator<GoodReceiptPoHeader>, GoodReceiptPoHeaderValidator>();
        services.AddScoped<IValidator<GoodReceiptPoLine>, GoodReceiptPoLineValidator>();
        services.AddScoped<IValidator<DeliveryOrderHeader>, DeliveryOrderHeaderValidator>();
        services.AddScoped<IValidator<DeliveryOrderLine>, DeliveryOrderLineValidator>();
        #endregion
        return services;
    }
}
