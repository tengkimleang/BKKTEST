
using Tri_Wall.API;
using Tri_Wall.Application;
using Tri_Wall.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddEndpointDefinitions(typeof(IEndpointDefinition));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSeriaizerContext.Default);
});

var app = builder.Build();
app.UseEndpointDefinitions();

app.Run();
