using System.Security.Claims;
using Tri_Wall.Application;
using Tri_Wall.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication().AddCookie("cookies");

builder.Services.AddCors(options =>
{
    // options.AddPolicy("CorsPolicy", policy =>
    // {
    //     policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5120");
    // });
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
// app.UseCors("CorsPolicy");
var api = app.MapGroup("api");
api.MapGet("/user", (ClaimsPrincipal user)
    => user.Claims.ToDictionary(x => x.Type, x => x.Value));
api.MapPost("/login", () => Results.SignIn(new ClaimsPrincipal(
        new ClaimsIdentity(
            new[] { new Claim("id", Guid.NewGuid().ToString()) },
            "cookies"
        )),
    authenticationScheme: "cookies"
));
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();