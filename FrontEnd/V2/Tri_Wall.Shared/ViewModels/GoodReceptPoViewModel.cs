using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class GoodReceptPoViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    #region Data Member

    [ObservableProperty] GoodReceiptPoHeader _goodReceiptPoForm = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] ObservableCollection<Vendors> _vendors = loadMasterData.GetVendors;

    [ObservableProperty] ObservableCollection<ContactPersons> _contactPeople = loadMasterData.GetContactPersons;

    [ObservableProperty] ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty] ObservableCollection<VatGroups> _taxPurchases = loadMasterData.GetTaxPurchases;

    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCountPurchaseOrder = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _goodReceiptPoHeaderDetailByDocNums = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _goodReceiptPoLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _getPurchaseOrderLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetGennerateBatchSerial> _getGenerateBatchSerial = new();

    [ObservableProperty] Boolean _isView;

    #endregion

    #region Method

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
            (await apiService.GetSeries("20")).Data ?? new());
        Vendors = await CheckingValueT(Vendors, async () =>
            (await apiService.GetVendors()).Data ?? new());
        ContactPeople = await CheckingValueT(ContactPeople, async () =>
            (await apiService.GetContactPersons()).Data ?? new());
        Items = await CheckingValueT(Items, async () =>
            (await apiService.GetItems()).Data ?? new());
        TaxPurchases = await CheckingValueT(TaxPurchases, async () =>
            (await apiService.GetTaxPurchases()).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
            (await apiService.GetWarehouses()).Data ?? new());
        TotalItemCount = (await apiService.GetTotalItemCount("GoodReceiptPo")).Data ?? new();
        TotalItemCountPurchaseOrder = (await apiService.GetTotalItemCount("PurchaseOrder")).Data ?? new();
        IsView = true;
    }

    [RelayCommand]
    async Task Submit()
    {
        GoodReceiptPoForm.ContactPersonCode = "0";
        PostResponses = await apiService.PostGoodReceptPo(GoodReceiptPoForm);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GoodReceiptPoHeader", perPage)).Data ?? new();
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
    async Task OnGetGoodReceiptPoHeaderDeatialByDocNum(string docEntry)
    {
        GoodReceiptPoHeaderDetailByDocNums =
            (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry,
                "GET_GoodReceipt_PO_Header_Detail_By_DocNum")).Data ?? new();
        GoodReceiptPoLineByDocNums =
            (await apiService.GetLineByDocNum("GET_GoodReceipt_PO_Line_Detail_By_DocNum", docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialGoodReceipt")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocNum(string docEntry)
    {
        GetPurchaseOrderLineByDocNums =
            (await apiService.GetLineByDocNum("GET_PurchaseOrder_Line_Detail_By_DocNum", docEntry)).Data ?? new();
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