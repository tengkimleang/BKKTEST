using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Pages;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class ReceiptFromProductionOrderViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
     #region Data Member

    [ObservableProperty] IssueProductionHeader _issueProduction = new();
    
    [ObservableProperty] ObservableCollection<IssueProductionLine> _issueProductionLine = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty] ObservableCollection<GetProductionOrder> _getProductionOrder = new();
    
    [ObservableProperty] ObservableCollection<GetProductionOrderLines> _getProductionOrderLines = new();

    [ObservableProperty] Boolean _isView = false;
    
    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;
    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();
    [ObservableProperty]
    ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();
    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _goodReceiptPoHeaderDeatialByDocNums = new();
    [ObservableProperty]
    ObservableCollection<GoodReceiptPoLineByDocNum> _goodReceiptPoLineByDocNums = new();
    #endregion 

    #region Method

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
            (await apiService.GetSeries("59")).Data ?? new());
        GetProductionOrder = await CheckingValueT(GetProductionOrder, async () =>
            (await apiService.GetProductionOrders("GetForReceiptProduction")).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
            (await apiService.GetWarehouses()).Data ?? new());
        TotalItemCount = (await apiService.GetTotalItemCount("ReceiptFromProduction")).Data ?? new();
        IsView = true;
    }

    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostIssueProduction(IssueProduction);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("ReceiptForProduction", perPage)).Data ?? new();
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
            GetListData = (await apiService.GetListGoodReceiptPo("ReceiptForProduction", ""
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
            GetProductionOrderLines = new();
            GetProductionOrderLines = (await apiService.GetIssueProductionLines(docEntry)).Data ?? new();
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
            //todo
            GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode("OnGetBatchOrSerialInIssueForProduction", dictionary["ItemType"],dictionary["ItemCode"],dictionary["DocEntry"])).Data ?? new();
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
        GoodReceiptPoHeaderDeatialByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry,"GET_ReceiptForProduction_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums = (await apiService.GetLineByDocNum("GetReceiptForProductionLineDetailByDocEntry",docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry,"GetBatchSerialReceiptForProduction")).Data ?? new();
    }
    #endregion
}