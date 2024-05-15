using System.Collections.ObjectModel;
using System.Net;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;

public class ApiService(IApiService apiService)
{
    public Task<ResponseData<ObservableCollection<Series>>> GetSeries(string SeriesNumber) 
        => apiService.GetSeries(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION","SERIES",SeriesNumber));
    public Task<ResponseData<ObservableCollection<Items>>> GetItems()
        => apiService.GetItems(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetItem"));
    public Task<ResponseData<ObservableCollection<Vendors>>> GetVendors()
        => apiService.GetVendors(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetVendor"));
    public Task<ResponseData<ObservableCollection<ContactPerson>>> GetContactPersons()
        => apiService.GetContactPersons(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetContactPersonByCardCode"));
}
