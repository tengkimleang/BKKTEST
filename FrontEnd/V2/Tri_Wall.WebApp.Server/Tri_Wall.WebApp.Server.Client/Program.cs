using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.ViewModels;
using Tri_Wall.WebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services
        .AddFluentUIComponents()
        .AddSingleton<IFormFactor, FormFactor>()
        .AddViewModels();
await builder.Build().RunAsync();
