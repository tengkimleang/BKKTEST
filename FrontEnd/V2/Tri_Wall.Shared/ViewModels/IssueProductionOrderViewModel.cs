﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class IssueProductionOrderViewModel(ApiService apiService) : ViewModelBase //, ILoadMasterData loadMasterData
{
     #region Data Member

    [ObservableProperty] IssueProductionHeader _issueProductionForm = new();
    
    [ObservableProperty] ObservableCollection<IssueProductionLine> _issueProductionLine = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty] ObservableCollection<GetProductionOrder> _getProductionOrder = new();
    
    [ObservableProperty] ObservableCollection<GetProductionOrderLines> _getProductionOrderLines = new();

    [ObservableProperty] Boolean _isView;

    [ObservableProperty] private ObservableCollection<Warehouses> _warehouses = new(); //= loadMasterData.GetWarehouses;

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _issueProductionHeaderDetailByDocNums = new();

    [ObservableProperty]  ObservableCollection<GoodReceiptPoLineByDocNum> _issueProductionLineByDocNums = new();
    [ObservableProperty] string _token = string.Empty;
    #endregion 

    #region Method

    [RelayCommand]
    async Task OnLoading()
    {
        // Series = await CheckingValueT(Series, async () =>
        //     (await apiService.GetSeries("60")).Data ?? new());
        GetProductionOrder = await CheckingValueT(GetProductionOrder, async () =>
            (await apiService.GetProductionOrders("GetForIssueProduction",Token)).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
            (await apiService.GetWarehouses(Token)).Data ?? new());
        await TotalCountIssueForProduction();
        IssueProductionForm.Series = Series.First().Code;
        IsView = true;
    }
    [RelayCommand]
    async Task TotalCountIssueForProduction()
    {
        TotalItemCount = (await apiService.GetTotalItemCount("IssueForProduction",Token)).Data ?? new();
    }

    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostIssueProduction(IssueProductionForm,Token);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("IssueForProduction", perPage,Token)).Data ?? new();
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
            GetListData = (await apiService.GetListGoodReceiptPo("IssueForProduction", ""
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
            GetListData = (await apiService.GetListGoodReceiptPo("GET_PURCHASE_ORDER", perPage,Token)).Data ?? new();
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
            GetProductionOrderLines = new();
            GetProductionOrderLines = (await apiService.GetProductionOrderLines(docEntry,Token)).Data ?? new();
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
            GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode("OnGetBatchOrSerialAvailableByItemCode", dictionary["ItemType"],dictionary["ItemCode"],Token)).Data ?? new();
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
        IssueProductionHeaderDetailByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry,"GET_IssueForProduction_Header_Detail_By_DocNum",Token)).Data ?? new();
        IssueProductionLineByDocNums = (await apiService.GetLineByDocNum("GetIssueForProductionLineDetailByDocEntry",docEntry,Token)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry,"GetBatchSerialIssueForProduction",Token)).Data ?? new();
    }
    #endregion
}