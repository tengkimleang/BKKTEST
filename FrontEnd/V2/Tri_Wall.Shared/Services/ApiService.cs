using System.Collections.ObjectModel;
using System.Net;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Models.InventoryTransfer;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Models.ReturnComponentProduction;

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
    public Task<ResponseData<ObservableCollection<GetListData>>> GetListGoodReceiptPo(string storeType,string perPage
        ,string type="",string dateFrom="",string dateTo="",string docNum="")
        => apiService.GetListGoodReceiptPo(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", storeType,perPage,type,dateFrom,dateTo,docNum));
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
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerial(string docEntry,string type,string lineNum="")
        => apiService.GetBatchOrSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", type, docEntry,lineNum));
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerialByItemCode(string storeType,string itemType,string itemCode,string docEntry="")
        => apiService.GetBatchOrSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", storeType, itemType,itemCode,docEntry));
    public Task<ResponseData<ObservableCollection<GetGennerateBatchSerial>>> GennerateBatchSerial(string itemCode,string qty)
        => apiService.GennerateBatchSerial(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GennerateBatchOrSerial", itemCode,qty));
    public Task<ResponseData<ObservableCollection<GetProductionOrder>>> GetProductionOrders(string type)
        => apiService.GetProductionOrders(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GET_Production_Order",type));
    public Task<ResponseData<ObservableCollection<GetProductionOrderLines>>> GetProductionOrderLines(string docEntry)
        => apiService.GetProductionOrderLines(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GET_Production_Order_Lines",docEntry));
    public Task<PostResponse> PostIssueProduction(IssueProductionHeader issueProductionHeader)
        => apiService.PostIssueProduction(issueProductionHeader);
    public Task<ResponseData<ObservableCollection<GetProductionOrderLines>>> GetIssueProductionLines(string docEntry)
        => apiService.GetProductionOrderLines(new GetRequest(
            "_USP_CALLTRANS_EWTRANSACTION", "GET_Issue_Production_Lines",docEntry));
    public Task<PostResponse> PostReturnFromProduction(ReturnComponentProductionHeader issueProductionHeader)
        => apiService.PostReturnFromProduction(issueProductionHeader);
    public Task<PostResponse> PostInventoryTransfer(InventoryTransferHeader inventoryTransfer)
        => apiService.PostInventoryTransfer(inventoryTransfer);
    public Task<PostResponse> PostReturn(DeliveryOrderHeader deliveryOrderHeader)
        => apiService.PostReturn(deliveryOrderHeader);
}
