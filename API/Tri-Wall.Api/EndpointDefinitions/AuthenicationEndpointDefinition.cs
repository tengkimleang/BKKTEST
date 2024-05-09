
using MediatR;

using Microsoft.AspNetCore.Identity.Data;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class AuthenicationEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapGroup("/auth");
            app.MapPost("/register", Register);
        }

        public void DefineServices(IServiceCollection services)
        {
        }
        internal async Task<IResult> Register(ISender mediator, RegisterRequest request)
        {
            return Results.Ok(null);
        }
    }
}
