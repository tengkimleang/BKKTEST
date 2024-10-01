using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Shared;
using Tri_Wall.Shared.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services
        .AddFluentUIComponents()
        // .AddScoped<IFormFactor, FormFactor>()
        .AddViewModels();
builder.Services.AddSingleton<ILoadMasterData, LoadMasterData>();
// builder.Services.AddHostedService<LoadMasterDataService>();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
