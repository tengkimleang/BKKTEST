using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Tri_Wall.Shared.Services;

public class CookieAuthenticationSateProvider(IApiService ApiService) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserStateAsync();
        return new AuthenticationState(user);
    }
    //public override Task<AuthenticationState> GetAuthenticationStateAsync()
    //{
    //    var identity = new ClaimsIdentity();
    //    var user = new ClaimsPrincipal(identity);

    //    return Task.FromResult(new AuthenticationState(user));
    //}
    public async Task Login()
    {
        var user = await ApiService.PostUser(new { });
        //var identity = new ClaimsIdentity(new[]
        //{
        //    new Claim(ClaimTypes.Name, "test"),
        //}, "Custom Authentication");

        //var user = new ClaimsPrincipal(identity);

        //NotifyAuthenticationStateChanged(
        //    Task.FromResult(new AuthenticationState(await GetUserStateAsync())));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    private async Task<ClaimsPrincipal> GetUserStateAsync()
    {
       // var user = await ApiService.GetUser();
        //if (!string.IsNullOrEmpty(user.Id))
        //{
        //    return new ClaimsPrincipal(new ClaimsIdentity());
        //}

        var claims = new List<Claim>
        {
            new Claim("id", "asdasd"),
            // Add other claims as needed, e.g.:
            // new Claim(ClaimTypes.Name, user.Name),
            // new Claim(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, "CustomAuthentication");
        return new ClaimsPrincipal(identity);
    }
}
