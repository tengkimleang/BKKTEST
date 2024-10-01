﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.API.Controllers;
using Tri_Wall.Application.GoodReturn;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[Route("/goodReturn")]
public class GoodReturnController(ISender mediator, IValidator<AddGoodReturnCommand> validator)
    : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(AddGoodReturnCommand command)
    {
        var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            return BadRequest(new PostResponse(
                ErrorMsg: validationResult.Errors[0].ErrorMessage,
                ErrorCode: StatusCodes.Status400BadRequest.ToString()));
        }

        var getData = await mediator.Send(command);
        return getData.Match<IActionResult>(
            data => Ok(data),
            err => BadRequest(new PostResponse
            {
                ErrorCode = err[0].Code,
                ErrorMsg = err[0].Description
            }));
    }
}
