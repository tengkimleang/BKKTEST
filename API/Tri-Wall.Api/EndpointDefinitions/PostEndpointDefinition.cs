
using FluentValidation;
using MediatR;
using Tri_Wall.Application;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Application.SaleOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class PostEndpointDefinition : IEndpointDefinition
    {
        public void DefineServices(IServiceCollection services)
        {
            services.AddScoped<IValidator<AddDeliveryOrderCommand>, AddDeliveryOrderCommandValidator>();
            services.AddScoped<IValidator<AddSaleOrderCommand>, AddSaleOrderCommandValidator>();
        }
        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("/deliveryOrder", CreateDeliveryOrder);
            app.MapPost("/saleOrder", CreateSaleOrder);
        }

        internal async Task<IResult> CreateDeliveryOrder(
            ISender mediator,
            AddDeliveryOrderCommand command,
            IValidator<AddDeliveryOrderCommand> validator)
        {
            var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.BadRequest(new PostResponse(
                    ErrorMsg: validationResult.Errors[0].ErrorMessage,
                    ErrorCode: StatusCodes.Status400BadRequest.ToString()));

            return (await mediator.Send(command).ConfigureAwait(false)).Match(
                data => Results.Ok(data),
                err => Results.BadRequest(new PostResponse(ErrorMsg: err[0].Description, ErrorCode: err[0].Code)));
        }
        internal async Task<IResult> CreateSaleOrder(
            ISender mediator,
            AddSaleOrderCommand command,
            IValidator<AddSaleOrderCommand> validator)
        {
            var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.BadRequest(new PostResponse(ErrorMsg: validationResult.Errors[0].ErrorMessage,ErrorCode: StatusCodes.Status400BadRequest.ToString()));

            return (await mediator.Send(command).ConfigureAwait(false)).Match(
                data => Results.Ok(data),
                err => Results.BadRequest(new PostResponse(ErrorMsg: err[0].Description, ErrorCode: err[0].Code)));
        }
    }
}
