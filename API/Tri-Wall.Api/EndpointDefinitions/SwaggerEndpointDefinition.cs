
namespace Tri_Wall.API.EndpointDefinitions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    /// <summary>
    /// Define the services for the Swagger endpoint
    /// </summary>
    /// <param name="app"></param>
    public void DefineEndpoints(WebApplication app)
    {
        //app.UseSwagger();
        //app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "GymManagement.Api v1"));
    }
    /// <summary>
    /// Define the services for the Swagger endpoint
    /// </summary>
    /// <param name="services"></param>
    public void DefineServices(IServiceCollection services)
    {
        //services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen(c =>
        //{
        //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GymManagement.Api", Version = "v1" });
        //});
    }
}
