using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.Application.Authorize;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[Route("/auth")]
public class AuthenicationController : ApiController
{
    private readonly ISender _mediator;
    private readonly IValidator<AuthorizeCommand> validator;
    public AuthenicationController(ISender mediator, IValidator<AuthorizeCommand> validator)
    {
        _mediator = mediator;
        this.validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Authenicate(AuthorizeCommand command)
    {
        var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
        if (!validationResult.IsValid)
            return BadRequest(new JwtResponse(ErrorMessage: validationResult.Errors[0].ErrorMessage, ErrorCode: StatusCodes.Status400BadRequest.ToString()));

        var getData = await _mediator.Send(command);
        return getData.Match<IActionResult>(
            data => Ok(data),
            err => BadRequest(new JwtResponse
            {
                ErrorCode = err[0].Code,
                ErrorMessage = err[0].Description
            }));
    }
}