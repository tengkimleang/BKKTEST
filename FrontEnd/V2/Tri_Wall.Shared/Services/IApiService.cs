using Refit;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;

namespace Tri_Wall.Shared.Services;

public interface IApiService
{
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Series>>> GetSeries(
        [Body]GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Items>>> GetItems(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Vendors>>> GetVendors(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<ContactPersons>>> GetContactPersons(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxPurchases(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Warehouses>>> GetWarehouses(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<TotalItemCount>>> GetTotalItemCount(
        [Body] GetRequest getRequest);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetListData>>> GetListGoodReceiptPo(
        [Body] GetRequest getRequest);
    [Post("/goodReceiptPo")]
    public Task<PostResponse> PostGoodReceptPo(
        [Body] GoodReceiptPoHeader request);
}