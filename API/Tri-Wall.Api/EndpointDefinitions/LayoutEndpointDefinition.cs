
using FluentValidation;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Application;
using Tri_Wall.Application.Layout;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions;

public class LayoutEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGroup("/create");
        app.MapPost("/layout", async (ISender mediator,
                LayoutCommand command,
                IValidator<LayoutCommand> validator) =>
        {
            var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new PrintViewLayoutResponse(
                    ErrorMessage: validationResult.Errors[0].ErrorMessage,
                    ErrCode: StatusCodes.Status400BadRequest.ToString()));
            }

            return (await mediator.Send(command).ConfigureAwait(false)).Match(
                data => Results.Ok(data),
                err => Results.BadRequest(new PrintViewLayoutResponse(ErrorMessage: err[0].Description, ErrCode: err[0].Code)));
        });
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddScoped<IValidator<LayoutCommand>, LayoutCommandValidator>();
    }
}
