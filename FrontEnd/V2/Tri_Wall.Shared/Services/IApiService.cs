using Refit;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;

public interface IApiService
{
    [Post("/getQuery")]
    public Task<ResponseData<List<Series>>> GetGetSeries(
        [Body]GetRequest getRequest);
}