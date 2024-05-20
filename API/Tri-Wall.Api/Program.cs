
using Tri_Wall.Application;
using Tri_Wall.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:7251");
    });
    //options.AddPolicy("AllowAll",
    //builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
