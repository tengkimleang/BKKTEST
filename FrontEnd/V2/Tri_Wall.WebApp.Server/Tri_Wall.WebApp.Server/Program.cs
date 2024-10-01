using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Shared;
using Tri_Wall.Shared.ViewModels;
using Tri_Wall.WebApp.Server.Components;
using Tri_Wall.WebApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddCookie("cookies");
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents()
    .AddScoped<IFormFactor, FormFactor>()
    .AddViewModels();
builder.Services.AddSingleton<ILoadMasterData, LoadMasterData>();
// builder.Services.AddHostedService<LoadMasterDataService>();
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

var api = app.MapGroup("api");
// api.MapGet("/user", (ClaimsPrincipal user)
//     => user.Claims.ToDictionary(x => x.Type, x => x.Value));
api.MapPost("/login",
    async (Dictionary<string, string> user, ApiAuthService apiAuthService, ClaimsPrincipal userClaimsPrincipal) =>
    {
        if (user is not { Count: > 0 })
        {
            return Results.Ok(userClaimsPrincipal.Claims.ToDictionary(x => x.Type, x => x.Value));
        }

        var result = await apiAuthService.CheckingUser(user["UserName"], user["Password"]);
        if (result.ErrorCode != "0")
        {
            return Results.Ok(result);
        }

        return Results.SignIn(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("userName", user["UserName"]),
                        new Claim("token", result.Token)
                    },
                    "cookies"
                )),
            authenticationScheme: "cookies"
        );
    });
api.MapPost("/logout",
    (Dictionary<string, string> user) =>
        Task.FromResult(Results.SignOut(new AuthenticationProperties { RedirectUri = "/" }, ["cookies"])));

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