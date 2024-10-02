using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using Tri_Wall.Application.Authorize;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[ApiController]
[Route("/auth")]
public class AuthenticationController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Authenicate(AuthorizeCommand command, IValidator<AuthorizeCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);
        if (!validationResult.IsValid)
            return BadRequest(new JwtResponse(ErrorMessage: validationResult.Errors[0].ErrorMessage, ErrorCode: StatusCodes.Status400BadRequest.ToString()));

        var getData = await mediator.Send(command);
        return getData.Match<IActionResult>(
            data => Ok(data),
            err => BadRequest(new JwtResponse
            {
                ErrorCode = err[0].Code,
                ErrorMessage = err[0].Description
            }));
    }
}