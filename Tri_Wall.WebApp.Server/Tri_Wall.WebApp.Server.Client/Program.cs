using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tri_Wall.Shared.Interfaces;
using Tri_Wall.WebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<IFormFactor, FormFactor>();
await builder.Build().RunAsync();
