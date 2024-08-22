using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.Application.Layout;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API.Controllers;

[Route("/layoutEndpoint")]
public class LayoutController(
    ISender mediator,
    IValidator<LayoutCommand> validator,
    IWebHostEnvironment webHostEnvironment)
    : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Create(string docEntry, string layoutCode)
    {
        LayoutCommand command = new()
        {
            DocEntry = docEntry,
            LayoutCode = layoutCode,
            StoreName = "_USP_CALLTRANS_EWTRANSACTION",
        };
        var validationResult = await validator.ValidateAsync(command).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            return BadRequest(new PostResponse(
                ErrorMsg: validationResult.Errors[0].ErrorMessage,
                ErrorCode: StatusCodes.Status400BadRequest.ToString()));
        }

        command.Path = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "Layouts");

        var getData = await mediator.Send(command);
        return getData.Match<IActionResult>(
            data => File(data.Data ?? [], data.ApplicationType, data.FileName),
            err => BadRequest(new PostResponse
            {
                ErrorCode = err[0].Code,
                ErrorMsg = err[0].Description
            }));
    }
}