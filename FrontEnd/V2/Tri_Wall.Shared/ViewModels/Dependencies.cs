using System.Net.Http.Headers;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
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

        // services.AddScoped<AuthenticatedHttpClientHandler>();
        services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                // client.BaseAddress = new Uri("http://localhost:5253");
                //client.BaseAddress = new Uri("http://localhost:8082");
                client.BaseAddress = new Uri("http://192.168.20.2:8082");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImZlMmZmZDc2LWJjOWUtNDFmZi05YmJjLTBmNTViNjJhOWUzNiIsImlkIjoiYWRtaW4xMC8wMi8yMDI0IDAyOjU0OjIwIiwibmJmIjoxNzI3ODM3NjYwLCJleHAiOjE3Mjg0NDI0NjAsImlhdCI6MTcyNzgzNzY2MH0.vkMAIEdI6AiPGJpLxR_8RA-p6nqnE-pM3XvBXuyalVY");
            }).AddStandardResilienceHandler(static options =>
                options.Retry = new WebOrMobileHttpRetryStrategyOptions());

        services.AddRefitClient<IApiAuthService>()
            .ConfigureHttpClient(static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                // client.BaseAddress = new Uri("http://localhost:5253");
                client.BaseAddress = new Uri("http://192.168.20.2:8082");
            })
            .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());

        #endregion

        #region Add ViewModel

        services.AddSingleton<ApiService>();
        services.AddSingleton<ApiAuthService>();
        services.AddScoped<GoodReceptPoViewModel>();
        services.AddScoped<DeliveryOrderViewModel>();
        services.AddScoped<InventoryTransferViewModel>();
        services.AddTransient<IssueProductionOrderViewModel>();
        services.AddTransient<ReturnFromComponentViewModel>();
        services.AddTransient<ReturnViewModel>();
        services.AddTransient<GoodReturnViewModel>();
        services.AddTransient<ArCreditMemoViewModel>();
        services.AddTransient<InventoryCountingViewModel>();
        services.AddScoped<ProductionProcessViewModel>();
        services.AddScoped<ReceiptsFinishedGoodsViewModel>();
        services.AddScoped<ReturnRequestViewModel>();

        #endregion

        #region Validator

        var assembly = Assembly.GetAssembly(typeof(Dependencies));
        var validatorTypes = assembly?.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        foreach (var validatorType in validatorTypes!)
        {
            var interfaceType = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddScoped(interfaceType, validatorType);
        }

        #endregion

        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5121"),
        });
        services.AddCascadingAuthenticationState();
        services.AddAuthorizationCore();
        services.AddScoped<CookieAuthenticationSateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CookieAuthenticationSateProvider>()
        );
        return services;
    }
}

public class GetToken
{
    public GetToken(CookieAuthenticationSateProvider cookieManager)
    {
        _ = GetTokenStatic(cookieManager);
    }

    public static string? Token { get; set; } = string.Empty;

    public async Task GetTokenStatic(CookieAuthenticationSateProvider cookieManager)
    {
        Token = string.Empty;
        var tokenCookie = await cookieManager.GetClaimAsync();
        Token = tokenCookie.FindFirst("token")?.Value;
    }
}