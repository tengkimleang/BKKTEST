
using MediatR;
using Tri_Wall.Application.Queries;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class GetEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapGroup("/get");
            app.MapPost("/getQuery", async (ISender mediator, GetAllQuery request) =>
            {
                var getData = await mediator.Send(request);
                return getData.Match(
                    data => Results.Ok(new GetResponse { Data = data }),
                    err => Results.BadRequest(new GetResponse
                    {
                        ErrorCode = err[0].Code,
                        ErrorMessage = err[0].Description
                    }));
            });
        }

        public void DefineServices(IServiceCollection services)
        {
        }
    }
}
