using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryTransfer;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class InventoryTransferViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    [ObservableProperty]
    InventoryTransferHeader _inventoryTransferForm = new();

    [ObservableProperty]
    ObservableCollection<Series> _series = new();

    [ObservableProperty]
    ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty]
    ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty]
    PostResponse _postResponses = new();

    [ObservableProperty]
    ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty]
    ObservableCollection<TotalItemCount> _totalItemCountSalesOrder = new();

    [ObservableProperty]
    ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _goodReceiptPoHeaderDeatialByDocNums = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoLineByDocNum> _goodReceiptPoLineByDocNums = new();

    [ObservableProperty]
    ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoLineByDocNum> _getPurchaseOrderLineByDocNums = new();

    [ObservableProperty]
    ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty]
    Boolean _isView = false;

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
                 (await apiService.GetSeries("67")).Data ?? new());
        Items = await CheckingValueT(Items, async () =>
                    (await apiService.GetItems()).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
                    (await apiService.GetWarehouses()).Data ?? new());
        TotalItemCount = (await apiService.GetTotalItemCount("InventoryTransfer")).Data ?? new();
        TotalItemCountSalesOrder = (await apiService.GetTotalItemCount("InventoryTransferRequest")).Data ?? new();
        IsView = true;
    }

    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostInventoryTransfer(InventoryTransferForm);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetDeliveryOrderHeader", perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [RelayCommand]
    async Task OnGetBatchOrSerialByItemCode(Dictionary<string, string> dictionary)
    {
        try
        {
            GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode("OnGetBatchOrSerialAvailableByItemCode", dictionary["ItemType"], dictionary["ItemCode"])).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [RelayCommand]
    async Task OnGetPurchaseOrder(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetSaleOrder", perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPoHeaderDeatialByDocNum(string docEntry)
    {
        GoodReceiptPoHeaderDeatialByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry, "GET_DeliveryOrder_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums = (await apiService.GetLineByDocNum("GetDeliveryOrderLineDetailByDocEntry", docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialDeliveryOrder")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocNum(string docEntry)
    {
        GetPurchaseOrderLineByDocNums = (await apiService.GetLineByDocNum("GetSaleOrderLineDetailByDocEntry", docEntry)).Data ?? new();
    }
    [RelayCommand]
    async Task OnGetGoodReceiptPoBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GoodReceiptPoHeader", ""
                , "condition"
                , data["dateFrom"].ToString() ?? ""
                , data["dateTo"].ToString() ?? ""
                , data["docNum"].ToString() ?? "")).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
