using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.Application.GoodReceiptPo;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[Route("/goodReceiptPo")]
public class GoodReceiptPoController : ApiController
{
    private readonly ISender _mediator;
    private readonly IValidator<AddGoodReceiptPoCommand> validator;
    public GoodReceiptPoController(ISender mediator, IValidator<AddGoodReceiptPoCommand> validator)
    {
        _mediator = mediator;
        this.validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddGoodReceiptPoCommand command)
    {
        var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            return BadRequest(new PostResponse(
                ErrorMsg: validationResult.Errors[0].ErrorMessage,
                ErrorCode: StatusCodes.Status400BadRequest.ToString()));
        }

        var getData = await _mediator.Send(command);
        return getData.Match<IActionResult>(
            data => Ok(data),
            err => BadRequest(new PostResponse
            {
                ErrorCode = err[0].Code,
                ErrorMsg = err[0].Description
            }));
    }
}
