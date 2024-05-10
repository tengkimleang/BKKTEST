﻿
using FluentValidation;
using MediatR;
using Tri_Wall.Application.Authorize;
using Tri_Wall.Application.GoodReturn;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class AuthenicationEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapGroup("/create");
            app.MapPost("/auth", async (ISender mediator,
                AuthorizeCommand command,
                IValidator<AuthorizeCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new JwtResponse(ErrorMessage: validationResult.Errors[0].ErrorMessage, ErrorCode: StatusCodes.Status400BadRequest.ToString()));

                return (await mediator.Send(command).ConfigureAwait(false)).Match(
                    data => Results.Ok(data),
                    err => Results.BadRequest(new JwtResponse(ErrorMessage: err[0].Description, ErrorCode: err[0].Code)));
            });
        }

        public void DefineServices(IServiceCollection services)
        {
            services.AddScoped<IValidator<AuthorizeCommand>, AuthorizeCommandValidator>();
        }
    }
}
