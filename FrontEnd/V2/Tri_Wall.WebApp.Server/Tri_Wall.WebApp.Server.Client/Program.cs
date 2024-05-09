using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Interfaces;
using Tri_Wall.WebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services
        .AddFluentUIComponents()
        .AddSingleton<IFormFactor, FormFactor>();
await builder.Build().RunAsync();
