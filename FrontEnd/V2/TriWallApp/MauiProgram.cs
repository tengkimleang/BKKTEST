using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared;
using Tri_Wall.Shared.Interfaces;
using TriWallApp.Services;

namespace TriWallApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //Ignore the interactive render settings in the shared razor class library by calling this method. 
            InteractiveRenderSettings.ConfigureBlazorHybridRenderModes();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddFluentUIComponents();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            // Add device specific services used by Razor Class Library (MyApp.Shared)
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            return builder.Build();
        }
    }
}
