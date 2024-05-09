
using MediatR;
using Tri_Wall.Application.DeliveryOrder;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class PostEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("/deliveryOrder", CreateDeliveryOrder);
        }
        internal async Task<IResult> CreateDeliveryOrder(ISender mediator,AddDeliveryOrderCommand command)
        {
            return (await mediator.Send(command)).Match(
                data =>Results.Ok(data),
                err => Results.BadRequest(err[0]));
        }
        public void DefineServices(IServiceCollection services)
        {
        }
    }
}
