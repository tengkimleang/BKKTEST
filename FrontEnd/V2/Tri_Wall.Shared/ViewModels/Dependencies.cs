using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Reflection;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public static class Dependencies
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        #region Add Refit Client
        services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                // client.BaseAddress = new Uri("http://localhost:5253");
                client.BaseAddress = new Uri("http://localhost:8082");
            })
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
        #endregion
        #region Add ViewModel
        services.AddSingleton<ApiService>();
        services.AddScoped<GoodReceptPoViewModel>();
        services.AddScoped<DeliveryOrderViewModel>();
        services.AddScoped<InventoryTransferViewModel>();
        services.AddTransient<IssueProductionOrderViewModel>();
        services.AddTransient<ReceiptFromProductionOrderViewModel>();
        services.AddTransient<ReturnViewModel>();
        services.AddTransient<GoodReturnViewModel>();
        services.AddTransient<ArCreditMemoViewModel>();
        services.AddTransient<InventoryCountingViewModel>();
        #endregion
        #region Validator
        var assembly = Assembly.GetAssembly(typeof(Dependencies));
        var validatorTypes = assembly?.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        foreach (var validatorType in validatorTypes!)
        {
            var interfaceType = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddScoped(interfaceType, validatorType);
        }
        #endregion
        return services;
    }
}
