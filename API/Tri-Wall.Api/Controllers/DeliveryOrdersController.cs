using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[Route("/deliveryOrders")]
public class DeliveryOrdersController(ISender mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(AddDeliveryOrderCommand command, IValidator<AddDeliveryOrderCommand> validator)
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
