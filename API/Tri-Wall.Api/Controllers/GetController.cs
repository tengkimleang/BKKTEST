using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tri_Wall.Application.Queries;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.API;

[Route("/getQuery")]
public class GetController(ISender mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Get(GetAllQuery request)
    {
        var getData = await mediator.Send(request);
        return getData.Match<IActionResult>(
            data => Ok(new GetResponse { Data = data }),
            err => BadRequest(new GetResponse
            {
                ErrorCode = err[0].Code,
                ErrorMessage = err[0].Description
            }));
    }
}
