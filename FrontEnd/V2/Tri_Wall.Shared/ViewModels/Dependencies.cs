using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Reflection;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Models.IssueForProduction;
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
                client.BaseAddress = new Uri("http://localhost:5253");
            })
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
        services.AddSingleton<ApiService>();
        services.AddScoped<GoodReceptPoViewModel>();
        services.AddScoped<DeliveryOrderViewModel>();
        services.AddScoped<IssueProductionOrderViewModel>();
        services.AddScoped<ReceiptFromProductionOrderViewModel>();
        #region Validator
        //services.AddScoped<IValidator<GoodReceiptPoHeader>, GoodReceiptPoHeaderValidator>();
        //services.AddScoped<IValidator<GoodReceiptPoLine>, GoodReceiptPoLineValidator>();
        //services.AddScoped<IValidator<DeliveryOrderHeader>, DeliveryOrderHeaderValidator>();
        //services.AddScoped<IValidator<DeliveryOrderLine>, DeliveryOrderLineValidator>();
        //services.AddScoped<IValidator<IssueProductionHeader>, IssueProductionHeaderValidator>();
        //services.AddScoped<IValidator<IssueProductionLine>, IssueProductionLineValidator>();
        var assembly = Assembly.GetAssembly(typeof(Dependencies)); // Replace DependencyInjection with any class within your target assembly
        var validatorTypes = assembly?.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        foreach (var validatorType in validatorTypes)
        {
            var interfaceType = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddScoped(interfaceType, validatorType);
        }
        #endregion
        return services;
    }
}
