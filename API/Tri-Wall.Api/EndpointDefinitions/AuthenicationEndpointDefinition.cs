
using FluentValidation;
using MediatR;
using Tri_Wall.Application.Authorize;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.EndpointDefinitions
{
    public class AuthenicationEndpointDefinition : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("/auth", Register);
        }

        public void DefineServices(IServiceCollection services)
        {
            services.AddScoped<IValidator<AuthorizeCommand>, AuthorizeCommandValidator>();
        }
        internal async Task<IResult> Register(ISender mediator, AuthorizeCommand request, IValidator<AuthorizeCommand> validator)
        {
            var validationResult = await validator.ValidateAsync(request).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.BadRequest(new JwtResponse(
                    ErrorMessage: validationResult.Errors[0].ErrorMessage,
                    ErrorCode: StatusCodes.Status400BadRequest.ToString()));

            return (await mediator.Send(request).ConfigureAwait(false)).Match(
                data => Results.Ok(data),
                err => Results.BadRequest(new PostResponse(ErrorMsg: err[0].Description, ErrorCode: err[0].Code)));
        }
    }
}
