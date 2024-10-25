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
        [Body]GetRequest getRequest, [Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Items>>> GetItems(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Vendors>>> GetVendors(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<ContactPersons>>> GetContactPersons(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxPurchases(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<VatGroups>>> GetTaxSales(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<Warehouses>>> GetWarehouses(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<TotalItemCount>>> GetTotalItemCount(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetListData>>> GetListGoodReceiptPo(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/goodReceiptPo")]
    public Task<PostResponse> PostGoodReceptPo(
        [Body] GoodReceiptPoHeader request,[Authorize()] string token);
    [Post("/deliveryOrders")]
    public Task<PostResponse> PostDelveryOrder(
        [Body] DeliveryOrderHeader request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum>>> GoodReceiptPoHeaderDeatialByDocNum(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GoodReceiptPoLineByDocNum>>> GetLineByDocNum(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetBatchOrSerial>>> GetBatchOrSerial(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetGennerateBatchSerial>>> GennerateBatchSerial(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetProductionOrder>>> GetProductionOrders(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetProductionOrderLines>>> GetProductionOrderLines(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetInventoryCountingList>>> GetInventoryCountingLists(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetInventoryCountingLines>>> GetInventoryCountingLines(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/issueForProduction")]
    public Task<PostResponse> PostIssueProduction(
        [Body] IssueProductionHeader request,[Authorize()] string token);
    [Post("/receiptFromProduction/returnComponent")]
    public Task<PostResponse> PostReturnFromProduction(
        [Body] ReturnComponentProductionHeader request,[Authorize()] string token);
    [Post("/inventoryTransfer")]
    public Task<PostResponse> PostInventoryTransfer(
        [Body] InventoryTransferHeader request,[Authorize()] string token);
    [Post("/return")]
    public Task<PostResponse> PostReturn(
        [Body] DeliveryOrderHeader request,[Authorize()] string token);
    [Post("/goodReturn")]
    public Task<PostResponse> PostGoodReturn(
        [Body] DeliveryOrderHeader request,[Authorize()] string token);
    [Post("/arCreditMemo")]
    public Task<PostResponse> PostArCreditMemo(
        [Body] DeliveryOrderHeader request,[Authorize()] string token);
    [Post("/inventoryCounting")]
    public Task<PostResponse> PostInventoryCounting(
        [Body] InventoryCountingHeader request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetDetailInventoryCountingHeaderByDocNum>>> GetDetailInventoryCountingHeaderByDocNum(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetDetailInventoryCountingLineByDocNum>>> GetDetailInventoryCountingLineByDocNum(
        [Body] GetRequest request,[Authorize()] string token);
    [Post("/receiptFromProduction/updateProcessProduction")]
    public Task<PostResponse> UpdateProcessProduction(
        [Body] ProductionProcessHeader request,[Authorize()] string token);
    [Post("/receiptFromProduction/receiptFinishGood")]
    public Task<PostResponse> PostReceiptFinishGood(
        [Body] ReceiptFinishGoodHeader request,[Authorize()] string token);
    [Post("/returnRequest")]
    public Task<PostResponse> PostReturnRequest(
        [Body] DeliveryOrderHeader request,[Authorize()] string token);
    [Post("/getQuery")]
    public Task<ResponseData<ObservableCollection<GetLayout>>> GetLayoutPrinter(
        [Body] GetRequest getRequest,[Authorize()] string token);
    [Post("/auth")]
    public Task<CheckUserResponse> CheckingUser(
        [Body] CreateUser getRequest);
}