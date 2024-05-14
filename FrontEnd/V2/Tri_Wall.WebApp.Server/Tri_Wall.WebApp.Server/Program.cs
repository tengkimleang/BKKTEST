using Microsoft.Extensions.Http.Resilience;
using Microsoft.FluentUI.AspNetCore.Components;
using Polly;
using Refit;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.ViewModels;
using Tri_Wall.WebApp.Server.Components;
using Tri_Wall.WebApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents()
    .AddScoped<IFormFactor, FormFactor>()
    .AddViewModels();

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
