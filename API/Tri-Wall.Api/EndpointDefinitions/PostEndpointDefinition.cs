
using MediatR;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class PostEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("/deliveryOrder", CreateDeliveryOrder);
        }
        internal async Task<IResult> CreateDeliveryOrder(ISender mediator, AddDeliveryOrderCommand command)
        {
            return (await mediator.Send(command)).Match(
                data => Results.Ok(data),
                err => Results.BadRequest(new PostResponse(ErrorMsg: err[0].Description, ErrorCode: err[0].Code)));
        }
        public void DefineServices(IServiceCollection services)
        {
        }
    }
}
