
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;
using static Microsoft.FluentUI.AspNetCore.Components.Emojis.Symbols.Color.Default;

namespace Tri_Wall.Shared.ViewModels;

public partial class ReturnViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    [ObservableProperty]
    DeliveryOrderHeader _deliveryOrderForm = new();

    [ObservableProperty]
    ObservableCollection<Series> _series = new();

    [ObservableProperty]
    ObservableCollection<Vendors> _customers = loadMasterData.GetCustomers;

    [ObservableProperty]
    ObservableCollection<ContactPersons> _contactPeople = loadMasterData.GetContactPersons;

    [ObservableProperty]
    ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty]
    ObservableCollection<VatGroups> _taxSales = loadMasterData.GetTaxSales;

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
    ObservableCollection<GetBatchOrSerial> _getBatchOrSerialReturns = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoLineByDocNum> _getPurchaseOrderLineByDocNums = new();

    [ObservableProperty]
    ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty]
    Boolean _isView = false;

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
                 (await apiService.GetSeries("16")).Data ?? new());
        Customers = await CheckingValueT(Customers, async () =>
                        (await apiService.GetCustomers()).Data ?? new());
        ContactPeople = await CheckingValueT(ContactPeople, async () =>
                    (await apiService.GetContactPersons()).Data ?? new());
        Items = await CheckingValueT(Items, async () =>
                    (await apiService.GetItems()).Data ?? new());
        TaxSales = await CheckingValueT(TaxSales, async () =>
                    (await apiService.GetTaxSales()).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
                    (await apiService.GetWarehouses()).Data ?? new());
        TotalItemCount = (await apiService.GetTotalItemCount("Return")).Data ?? new();
        TotalItemCountSalesOrder = (await apiService.GetTotalItemCount("DeliveryOrderReturn")).Data ?? new();
        IsView = true;
    }

    [RelayCommand]
    async Task Submit()
    {
        DeliveryOrderForm.ContactPersonCode = "0";
        PostResponses = await apiService.PostReturn(DeliveryOrderForm);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetReturnHeader", perPage)).Data ?? new();
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
            GetBatchOrSerialsByItemCode = (
                await apiService.GetBatchOrSerialByItemCode(
                    "OnGetBatchOrSerialAvailableByItemCode", 
                    dictionary["ItemType"], 
                    dictionary["ItemCode"])
                ).Data ?? new();
            //if(dictionary["DocEntry"]=="" && dictionary["DocEntry"] == "0")
            //{
            //}
            //else
            //{
            //    GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode(
            //        "OnGetBatchOrSerialByItemCodeReuturnDelivery", 
            //        dictionary["ItemType"], 
            //        dictionary["ItemCode"], 
            //        dictionary["DocEntry"])).Data ?? new();
            //}
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
            GetListData = (await apiService.GetListGoodReceiptPo("GetDeliveryOrderReturn", perPage)).Data ?? new();
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
        GoodReceiptPoHeaderDeatialByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry, "GET_Return_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums = (await apiService.GetLineByDocNum("GetReturnLineDetailByDocEntry", docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialReturn")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocNum(string docEntry)
    {
        GetPurchaseOrderLineByDocNums = (await apiService.GetLineByDocNum("GetDeliveryOrderLineForReturnDetailByDocEntry", docEntry)).Data ?? new();
        //foreach(var obj in GetPurchaseOrderLineByDocNums)
        //{
        //    obj.Batches = (await apiService.GetLineByDocNum("GetDeliveryOrderLineForReturnDetailByDocEntry", docEntry)).Data ?? new();
        //}
    }
    [RelayCommand]
    async Task OnGetGoodReceiptPoBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GoodReturnDoHeader", ""
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