using Refit;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Models.InventoryCounting;
using Tri_Wall.Shared.Models.InventoryTransfer;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Models.ProductionProcess;
using Tri_Wall.Shared.Models.ReceiptFinishGood;
using Tri_Wall.Shared.Models.ReturnComponentProduction;
using Tri_Wall.Shared.Models.User;

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
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetProductionOrder>>> GetProductionOrders(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetProductionOrderLines>>> GetProductionOrderLines(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetInventoryCountingList>>> GetInventoryCountingLists(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetInventoryCountingLines>>> GetInventoryCountingLines(
        [Body] GetRequest request);
    [Post("/issueForProduction")]
    public Task<PostResponse> PostIssueProduction(
        [Body] IssueProductionHeader request);
    [Post("/receiptFromProduction/returnComponent")]
    public Task<PostResponse> PostReturnFromProduction(
        [Body] ReturnComponentProductionHeader request);
    [Post("/inventoryTransfer")]
    public Task<PostResponse> PostInventoryTransfer(
        [Body] InventoryTransferHeader request);
    [Post("/return")]
    public Task<PostResponse> PostReturn(
        [Body] DeliveryOrderHeader request);
    [Post("/goodReturn")]
    public Task<PostResponse> PostGoodReturn(
        [Body] DeliveryOrderHeader request);
    [Post("/arCreditMemo")]
    public Task<PostResponse> PostARCreditMemo(
        [Body] DeliveryOrderHeader request);
    [Post("/inventoryCounting")]
    public Task<PostResponse> PostInventoryCounting(
        [Body] InventoryCountingHeader request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetDetailInventoryCountingHeaderByDocNum>>> GetDetailInventoryCountingHeaderByDocNum(
        [Body] GetRequest request);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetDetailInventoryCountingLineByDocNum>>> GetDetailInventoryCountingLineByDocNum(
        [Body] GetRequest request);
    [Post("/api/login")]
    public Task<Dictionary<string, string>> PostUser(object request);
    [Get("/api/user")]
    public Task<GetAuthTest> GetUser();
    [Post("/receiptFromProduction/updateProcessProduction")]
    public Task<PostResponse> UpdateProcessProduction(
        [Body] ProductionProcessHeader request);
    [Post("/receiptFromProduction/receiptFinishGood")]
    public Task<PostResponse> PostReceiptFinishGood(
        [Body] ReceiptFinishGoodHeader request);
    [Post("/returnRequest")]
    public Task<PostResponse> PostReturnRequest(
        [Body] DeliveryOrderHeader request);
}

public interface IApiAuthService
{
    [Post("/auth")]
    public Task<CheckUserResponse> CheckingUser(
        [Body] CreateUser getRequest);
}