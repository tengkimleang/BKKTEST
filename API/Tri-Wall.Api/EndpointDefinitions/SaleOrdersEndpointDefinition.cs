
using FluentValidation;
using MediatR;
using Tri_Wall.Application.SaleOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class SaleOrdersEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapGroup("/create");
            app.MapPost("/saleOrder", async (ISender mediator,
                AddSaleOrderCommand command,
                IValidator<AddSaleOrderCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new PostResponse(ErrorMsg: validationResult.Errors[0].ErrorMessage, ErrorCode: StatusCodes.Status400BadRequest.ToString()));

                return (await mediator.Send(command).ConfigureAwait(false)).Match(
                    data => Results.Ok(data),
                    err => Results.BadRequest(new PostResponse(ErrorMsg: err[0].Description, ErrorCode: err[0].Code)));
            });
        }

        public void DefineServices(IServiceCollection services)
        {
            services.AddScoped<IValidator<AddSaleOrderCommand>, AddSaleOrderCommandValidator>();
        }
    }
}
