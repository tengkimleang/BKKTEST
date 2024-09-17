﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.ReceiptFinishGood;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class ReceiptsFinishedGoodsViewModel(ApiService apiService, ILoadMasterData loadMasterData)
    : ViewModelBase
{
    #region Data Member

    [ObservableProperty] ReceiptFinishGoodHeader _receiptFromProductionOrderForm = new();

    [ObservableProperty] ObservableCollection<ReceiptFinishGoodLine> _issueProductionLine = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty] ObservableCollection<GetProductionOrder> _getProductionOrder = new();

    [ObservableProperty] ObservableCollection<GetProductionOrderLines> _getProductionOrderLines = new();

    [ObservableProperty] Boolean _isView;

    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;
    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _goodReceiptPoHeaderDetailByDocNums = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _goodReceiptPoLineByDocNums = new();
    [ObservableProperty] ObservableCollection<GetGennerateBatchSerial> _getGenerateBatchSerial = new();

    #endregion

    #region Method

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
            (await apiService.GetSeries("59")).Data ?? new());
        GetProductionOrder = await CheckingValueT(GetProductionOrder, async () =>
            (await apiService.GetProductionOrders("GetProductionForFinishGoods")).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
            (await apiService.GetWarehouses()).Data ?? new());
        ReceiptFromProductionOrderForm.Series = Series.First().Code;
        await TotalCountReceiptFromProduction();
        IsView = true;
    }

    [RelayCommand]
    async Task TotalCountReceiptFromProduction()
    {
        TotalItemCount = (await apiService.GetTotalItemCount("ReceiptFromProduction")).Data ?? new();
    }
    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostReceiptFinishGood(ReceiptFromProductionOrderForm);
    }

    [RelayCommand]
    async Task OnGetProductionOrder()
    {
        GetProductionOrder = (await apiService.GetProductionOrders("GetProductionForFinishGoods")).Data ?? new();
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
            GetProductionOrderLines = (await apiService.GetProductionFinishedGoodLines(docEntry)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnIssueForProductionDetailByDocNum(string docEntry)
    {
        GoodReceiptPoHeaderDetailByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry,
            "GET_ReceiptForProduction_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums = (await apiService.GetLineByDocNum("GetReceiptForProductionLineDetailByDocEntry",
            docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry,
            "GetBatchSerialReceiptForProduction")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetGennerateBatchSerial(Dictionary<string, object> data)
    {
        try
        {
            GetGenerateBatchSerial = (await apiService.GennerateBatchSerial(data["itemCode"].ToString() ?? ""
                , data["qty"].ToString() ?? "")).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}