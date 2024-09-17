using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryCounting;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class InventoryCountingViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
     #region Data Member

    [ObservableProperty] InventoryCountingHeader _inventoryCountingHeader = new();
    
    [ObservableProperty] ObservableCollection<InventoryCountingLine> _inventoryCountingLines = new();

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty] ObservableCollection<GetInventoryCountingList> _getInventoryCountingLists = new();
    
    [ObservableProperty] ObservableCollection<GetInventoryCountingLines> _getInventoryCountingLines = new();

    [ObservableProperty] Boolean _isView;
    
    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty] ObservableCollection<GetDetailInventoryCountingHeaderByDocNum> _getDetailInventoryCountingHeaderByDocNums = new();

    [ObservableProperty]  ObservableCollection<GetDetailInventoryCountingLineByDocNum> _getDetailInventoryCountingLineByDocNums = new();
    #endregion 

    #region Method

    public override async Task Loaded()
    {
        GetInventoryCountingLists = await CheckingValueT(GetInventoryCountingLists, async () =>
            (await apiService.GetInventoryCountingLists("GetInventoryCountingList")).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
            (await apiService.GetWarehouses()).Data ?? new());
        await TotalCountInventoryCounting();
        IsView = true;
    }
    [RelayCommand]
    async Task TotalCountInventoryCounting()
    {
        TotalItemCount = (await apiService.GetTotalItemCount("InventoryCounting")).Data ?? new();
    }
    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostInventoryCounting(InventoryCountingHeader);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("InventoryCounting", perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPoBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("InventoryCounting", ""
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

    [RelayCommand]
    async Task OnGetPurchaseOrder(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GET_PURCHASE_ORDER", perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GET_PURCHASE_ORDER", ""
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
    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocEntry(string docEntry)
    {
        try
        {
            GetInventoryCountingLines = new();
            GetInventoryCountingLines = (await apiService.GetInventoryCountingLines(docEntry)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [RelayCommand]
    async Task OnGetBatchOrSerialByItemCode(Dictionary<string,string> dictionary)
    {
        try
        {
            GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode("OnGetBatchOrSerialAvailableByItemCode", dictionary["ItemType"],dictionary["ItemCode"])).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [RelayCommand]
    async Task OnIssueForProductionDeatialByDocNum(string docEntry)
    {
        GetDetailInventoryCountingHeaderByDocNums = (await apiService.GetDetailInventoryCountingHeaderByDocNum(docEntry)).Data ?? new();
        GetDetailInventoryCountingLineByDocNums = (await apiService.GetDetailInventoryCountingLineByDocNum(docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialInventoryCounting")).Data ?? new();
    }
    #endregion
}