using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class DeliveryOrderViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    [ObservableProperty]
    DeliveryOrderHeader _DeliveryOrderForm=new();

    [ObservableProperty]
    ObservableCollection<Series> _series = new();

    [ObservableProperty]
    ObservableCollection<Vendors> _Customers = loadMasterData.GetCustomers;

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
    ObservableCollection<TotalItemCount> _totalItemCount=new();
    
    [ObservableProperty]
    ObservableCollection<TotalItemCount> _totalItemCountSalesOrder=new();

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

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
                 (await apiService.GetSeries("15")).Data ?? new());
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
        TotalItemCount = (await apiService.GetTotalItemCount("DeliveryOrder")).Data ?? new();
        TotalItemCountSalesOrder = (await apiService.GetTotalItemCount("SaleOrder")).Data ?? new();
    }

    [RelayCommand]
    async Task Submit()
    {
        DeliveryOrderForm.ContactPersonCode = "0";
        PostResponses = await apiService.PostDelveryOrder(DeliveryOrderForm);
    }
    
    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetDeliveryOrderHeader",perPage)).Data ?? new();
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
            GetBatchOrSerialsByItemCode = (await apiService.GetBatchOrSerialByItemCode("GetDeliveryOrderHeader",dictionary["ItemType"],dictionary["ItemCode"])).Data ?? new();
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
            GetListData = (await apiService.GetListGoodReceiptPo("GetSaleOrder",perPage)).Data ?? new();
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
        GoodReceiptPoHeaderDeatialByDocNums = (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry,"GET_DeliveryOrder_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums = (await apiService.GetLineByDocNum("GetDeliveryOrderLineDetailByDocEntry",docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry,"GetBatchSerialDeliveryOrder")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocNum(string docEntry)
    {
        GetPurchaseOrderLineByDocNums = (await apiService.GetLineByDocNum("GetSaleOrderLineDetailByDocEntry", docEntry)).Data ?? new();
    }
}