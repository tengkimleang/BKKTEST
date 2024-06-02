using System.Collections.ObjectModel;
using System.Net;
using Tri_Wall.Shared.Models;
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
    public Task<ResponseData<ObservableCollection<ContactPersons>>> GetContactPersons()
        => apiService.GetContactPersons(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetContactPersonByCardCode"));
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxPurchases()
        => apiService.GetTaxPurchases(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetVatCodePurchase"));
    public Task<ResponseData<ObservableCollection<Warehouses>>> GetWarehouses()
        => apiService.GetWarehouses(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetWarehouseMasterData"));
    public Task<ResponseData<ObservableCollection<TotalItemCount>>> GetTotalItemCount(string type)
        => apiService.GetTotalItemCount(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "TotalItemCount",type));
    public Task<ResponseData<ObservableCollection<GetListData>>> GetListGoodReceiptPo(string perPage)
        => apiService.GetListGoodReceiptPo(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GoodReceiptPoHeader",perPage));
    public Task<PostResponse> PostGoodReceptPo(GoodReceiptPoHeader goodReceiptPoHeader)
        => apiService.PostGoodReceptPo(goodReceiptPoHeader);
    public Task<ResponseData<ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum>>> GoodReceiptPoHeaderDeatialByDocNum(string docEntry)
        => apiService.GoodReceiptPoHeaderDeatialByDocNum(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GET_GoodReceipt_PO_Header_Detail_By_DocNum", docEntry));
    public Task<ResponseData<ObservableCollection<GoodReceiptPoLineByDocNum>>> GoodReceiptPoHeaderLineByDocNum(string docEntry)
        => apiService.GoodReceiptPoHeaderLineByDocNum(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GET_GoodReceipt_PO_Line_Detail_By_DocNum", docEntry));
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerial(string docEntry)
        => apiService.GetBatchOrSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GetBatchSerialGoodReceipt", docEntry));
}
