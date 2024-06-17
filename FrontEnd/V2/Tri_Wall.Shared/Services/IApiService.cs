using Refit;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
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
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxSales(
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
    [Post("/deliveryOrders")]
    public Task<PostResponse> PostDelveryOrder(
        [Body] DeliveryOrderHeader request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum>>> GoodReceiptPoHeaderDeatialByDocNum(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GoodReceiptPoLineByDocNum>>> GetLineByDocNum(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerial(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetGennerateBatchSerial>>> GennerateBatchSerial(
        [Body] GetRequest request);
}