using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class ArCreditMemoViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    #region Data Member

    [ObservableProperty] DeliveryOrderHeader _aRCreditMemoForm = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] ObservableCollection<Vendors> _customers = loadMasterData.GetCustomers;

    [ObservableProperty] ObservableCollection<ContactPersons> _contactPeople = loadMasterData.GetContactPersons;

    [ObservableProperty] ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty] ObservableCollection<VatGroups> _taxSales = loadMasterData.GetTaxSales;

    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCountArInvoice = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _aRCreditMemoHeaderDetailByDocNums = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _aRCreditMemoLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _getArInvoiceLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty] Boolean _isView;

    #endregion

    #region Method

    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
            (await apiService.GetSeries("14")).Data ?? new());
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
        TotalItemCount = (await apiService.GetTotalItemCount("ARCreditMemo")).Data ?? new();
        await OnTotalItemCountARCreditMemo();
        await OnTotalItemCountARInvoiceOpenStatus();
        ARCreditMemoForm.Series = Series.First().Code;
        IsView = true;
    }
    
    [RelayCommand]
    async Task OnTotalItemCountARInvoiceOpenStatus()
    {
        TotalItemCountArInvoice = (await apiService.GetTotalItemCount("ARInvoiceOpenStatus")).Data ?? new();
    }
    [RelayCommand]
    async Task OnTotalItemCountARCreditMemo()
    {
        TotalItemCount = (await apiService.GetTotalItemCount("ARCreditMemo")).Data ?? new();
    }
    
    [RelayCommand]
    async Task Submit()
    {
        ARCreditMemoForm.ContactPersonCode = string.IsNullOrEmpty(ARCreditMemoForm.ContactPersonCode) ? "0" : ARCreditMemoForm.ContactPersonCode;
        PostResponses = await apiService.PostArCreditMemo(ARCreditMemoForm);
    }

    [RelayCommand]
    async Task OnGetArCreditMemo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetARCreditMemoHeader", perPage)).Data ?? new();
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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnGetArInvoice(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetARInvoiceInCreditMemo", perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    async Task OnGetArCreditMemoHeaderDeatialByDocNum(string docEntry)
    {
        ARCreditMemoHeaderDetailByDocNums =
            (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry, "GET_AR_Credit_Memo_Header_Detail_By_DocNum"))
            .Data ?? new();
        ARCreditMemoLineByDocNums =
            (await apiService.GetLineByDocNum("GetARCreditMemoLineDetailByDocEntry", docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialARCreditMemo")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetArInvoiceLineByDocNum(string docEntry)
    {
        GetArInvoiceLineByDocNums =
            (await apiService.GetLineByDocNum("GetARInvoiceLineForARCreditMemoDetailByDocEntry", docEntry)).Data ??
            new();
        foreach (var obj in GetArInvoiceLineByDocNums)
        {
            if (obj.ManageItem == "S")
            {
                obj.Serials = new();
                var rs =
                    (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialARInvoiceForARCreditMemo",
                        obj.BaseLineNumber))
                    .Data ?? new();
                foreach (var objSerial in rs)
                {
                    obj.Serials.Add(new SerialGoodReceiptPoCopyFrom
                    {
                        MfrNo = objSerial.MfrSerialNo,
                        SerialCode = objSerial.SerialBatch,
                        Qty = Convert.ToInt32(objSerial.Qty),
                        MfrDate = string.IsNullOrEmpty(objSerial.MrfDate)
                            ? null
                            : Convert.ToDateTime(objSerial.MrfDate),
                        ExpDate = string.IsNullOrEmpty(objSerial.ExpDate)
                            ? null
                            : Convert.ToDateTime(objSerial.ExpDate),
                        OnSelectedBatchOrSerial = new[] { objSerial },
                    });
                }
            }
            else
            {
                obj.Batches = new();
                var rs =
                    (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialARInvoiceForARCreditMemo",
                        obj.BaseLineNumber))
                    .Data ?? new();
                foreach (var objBatch in rs)
                {
                    obj.Batches.Add(new BatchGoodReceiptPoCopyFrom
                    {
                        LotNo = objBatch.MfrSerialNo,
                        BatchCode = objBatch.SerialBatch,
                        Qty = Convert.ToDouble(objBatch.Qty),
                        ManfectureDate = string.IsNullOrEmpty(objBatch.MrfDate)
                            ? null
                            : Convert.ToDateTime(objBatch.MrfDate),
                        ExpDate = string.IsNullOrEmpty(objBatch.ExpDate)
                            ? null
                            : Convert.ToDateTime(objBatch.ExpDate),
                        QtyAvailable = Convert.ToDouble(objBatch.Qty),
                        OnSelectedBatchOrSerial = new[] { objBatch },
                    });
                }
            }
        }
    }

    [RelayCommand]
    async Task OnGetArCreditMemoBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("ARCreditMemoHeader", ""
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

    #endregion
}