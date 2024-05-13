using System.Net;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;

public class ApiService(IApiService apiService)
{
    public Task<ResponseData<List<Series>>> GetSeries(string SeriesNumber) 
        => apiService.GetGetSeries(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION","SERIES",SeriesNumber));
}
