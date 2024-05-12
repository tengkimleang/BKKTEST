using Microsoft.Extensions.Http.Resilience;
using Microsoft.FluentUI.AspNetCore.Components;
using Polly;
using Refit;
using Tri_Wall.Shared;
using Tri_Wall.Shared.Interfaces;
using Tri_Wall.WebApp.Server.Components;
using Tri_Wall.WebApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<IFormFactor, FormFactor>();
builder.Services.AddRefitClient<IApiService>()
    .ConfigureHttpClient(static client => client.BaseAddress = new Uri("http://localhost:5253"))
    .AddStandardResilienceHandler(static options => options.Retry = new WebOrMobileHttpRetryStrategyOptions());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Tri_Wall.Shared._Imports).Assembly);

app.Run();


sealed class WebOrMobileHttpRetryStrategyOptions : HttpRetryStrategyOptions
{
    public WebOrMobileHttpRetryStrategyOptions()
    {
        BackoffType = DelayBackoffType.Exponential;
        MaxRetryAttempts = 3;
        UseJitter = true;
        Delay = TimeSpan.FromSeconds(1.5);
    }
}