using System.Collections.ObjectModel;
using System.Net;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;

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
    public Task<ResponseData<ObservableCollection<Vendors>>> GetCustomers()
        => apiService.GetVendors(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetCustomer"));
    public Task<ResponseData<ObservableCollection<ContactPersons>>> GetContactPersons()
        => apiService.GetContactPersons(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetContactPersonByCardCode"));
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxPurchases()
        => apiService.GetTaxPurchases(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetVatCodePurchase"));
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxSales()
        => apiService.GetTaxSales(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetTaxSale"));
    public Task<ResponseData<ObservableCollection<Warehouses>>> GetWarehouses()
        => apiService.GetWarehouses(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetWarehouseMasterData"));
    public Task<ResponseData<ObservableCollection<TotalItemCount>>> GetTotalItemCount(string type)
        => apiService.GetTotalItemCount(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "TotalItemCount",type));
    public Task<ResponseData<ObservableCollection<GetListData>>> GetListGoodReceiptPo(string storeType,string perPage)
        => apiService.GetListGoodReceiptPo(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", storeType,perPage));
    public Task<PostResponse> PostGoodReceptPo(GoodReceiptPoHeader goodReceiptPoHeader)
        => apiService.PostGoodReceptPo(goodReceiptPoHeader);
    public Task<PostResponse> PostDelveryOrder(DeliveryOrderHeader deliveryOrderHeader)
        => apiService.PostDelveryOrder(deliveryOrderHeader);
    public Task<ResponseData<ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum>>> GoodReceiptPoHeaderDeatialByDocNum(string docEntry,string type)
        => apiService.GoodReceiptPoHeaderDeatialByDocNum(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", type, docEntry));
    public Task<ResponseData<ObservableCollection<GoodReceiptPoLineByDocNum>>> GetLineByDocNum(string storeType,string docEntry)
        => apiService.GetLineByDocNum(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", storeType, docEntry));
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerial(string docEntry,string type)
        => apiService.GetBatchOrSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", type, docEntry));
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerialByItemCode(string storeType,string itemType,string itemCode)
        => apiService.GetBatchOrSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", storeType, itemType,itemCode));
}
