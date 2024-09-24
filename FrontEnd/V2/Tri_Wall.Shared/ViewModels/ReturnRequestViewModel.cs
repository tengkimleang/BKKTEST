using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class ReturnRequestViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    #region Data Member
    [ObservableProperty] DeliveryOrderHeader _deliveryOrderForm = new();

    [ObservableProperty] ObservableCollection<Series> _series = new();

    [ObservableProperty] ObservableCollection<Vendors> _customers = loadMasterData.GetCustomers;

    [ObservableProperty] ObservableCollection<ContactPersons> _contactPeople = loadMasterData.GetContactPersons;

    [ObservableProperty] ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty] ObservableCollection<VatGroups> _taxSales = loadMasterData.GetTaxSales;

    [ObservableProperty] ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty] PostResponse _postResponses = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCount = new();

    [ObservableProperty] ObservableCollection<TotalItemCount> _totalItemCountDeliveryOrder = new();

    [ObservableProperty] ObservableCollection<GetListData> _getListData = new();

    [ObservableProperty]
    ObservableCollection<GoodReceiptPoHeaderDeatialByDocNum> _goodReceiptPoHeaderDetailByDocNums = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _goodReceiptPoLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerials = new();

    [ObservableProperty] ObservableCollection<GoodReceiptPoLineByDocNum> _getPurchaseOrderLineByDocNums = new();

    [ObservableProperty] ObservableCollection<GetBatchOrSerial> _getBatchOrSerialsByItemCode = new();

    [ObservableProperty] bool _isView;
    #endregion

    #region Method
    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
            (await apiService.GetSeries("234000031")).Data ?? new());
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
        await TotalCountReturn();
        await TotalCountDeliveryOrderReturn();
        DeliveryOrderForm.Series = Series.First().Code;
        IsView = true;
    }
    
    [RelayCommand]
    async Task TotalCountDeliveryOrderReturn()
    {
        TotalItemCountDeliveryOrder = (await apiService.GetTotalItemCount("DeliveryOrderReturnRequest")).Data ?? new();
    }
    
    [RelayCommand]
    async Task TotalCountReturn()
    {
        TotalItemCount = (await apiService.GetTotalItemCount("ReturnRequest")).Data ?? new();
    }

    [RelayCommand]
    async Task Submit()
    {
        DeliveryOrderForm.ContactPersonCode = string.IsNullOrEmpty(DeliveryOrderForm.ContactPersonCode) ? "0" : DeliveryOrderForm.ContactPersonCode;
        PostResponses = await apiService.PostReturnRequest(DeliveryOrderForm);
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("GetReturnRequestHeader", perPage)).Data ?? new();
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
            GetBatchOrSerialsByItemCode =
                (await apiService.GetBatchOrSerialByItemCode("OnGetBatchOrSerialAvailableByItemCode",
                    dictionary["ItemType"], dictionary["ItemCode"])).Data ?? new();
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
            GetListData = (await apiService.GetListGoodReceiptPo("GetDeliveryOrderReturnRequest", perPage)).Data ?? new();
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
            (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry, "GET_Return_Request_Header_Detail_By_DocNum"))
            .Data ?? new();
        GoodReceiptPoLineByDocNums =
            (await apiService.GetLineByDocNum("GetReturnRequestLineDetailByDocEntry", docEntry)).Data ?? new();
        GetBatchOrSerials = (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialReturnRequest")).Data ?? new();
    }

    [RelayCommand]
    async Task OnGetPurchaseOrderLineByDocNum(string docEntry)
    {
        GoodReceiptPoHeaderDetailByDocNums =
            (await apiService.GoodReceiptPoHeaderDeatialByDocNum(docEntry, "GET_Delivery_Order_Header_Detail_By_DocNum_Return_Request"))
            .Data ?? new();
        GetPurchaseOrderLineByDocNums =
            (await apiService.GetLineByDocNum("GetDeliveryOrderLineForReturnRequestDetailByDocEntry", docEntry)).Data ?? new();
        foreach (var obj in GetPurchaseOrderLineByDocNums)
        {
            if (obj.ManageItem == "S")
            {
                obj.Serials = new();
                var rs =
                    (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialDeliveryOrder", obj.BaseLineNumber))
                    .Data ?? new();
                foreach (var objSerial in rs)
                {
                    obj.Serials.Add(new SerialGoodReceiptPoCopyFrom
                    {
                        MfrNo = objSerial.MfrSerialNo,
                        SerialCode = objSerial.SerialBatch,
                        Qty = Convert.ToInt32(objSerial.Qty),
                        MfrDate = Convert.ToDateTime(objSerial.MrfDate),
                        ExpDate = Convert.ToDateTime(objSerial.ExpDate),
                        OnSelectedBatchOrSerial = new[] { objSerial },
                    });
                }
            }
            else
            {
                obj.Batches = new();
                var rs =
                    (await apiService.GetBatchOrSerial(docEntry, "GetBatchSerialDeliveryOrder", obj.BaseLineNumber))
                    .Data ?? new();
                foreach (var objBatch in rs)
                {
                    obj.Batches.Add(new BatchGoodReceiptPoCopyFrom
                    {
                        LotNo = objBatch.MfrSerialNo,
                        BatchCode = objBatch.SerialBatch,
                        Qty = Convert.ToDouble(objBatch.Qty),
                        ManfectureDate = Convert.ToDateTime(objBatch.MrfDate),
                        ExpDate = Convert.ToDateTime(objBatch.ExpDate),
                        QtyAvailable = Convert.ToDouble(objBatch.Qty),
                        OnSelectedBatchOrSerial = new[] { objBatch },
                    });
                }
            }
        }
    }

    [RelayCommand]
    async Task OnGetGoodReceiptPoBySearch(Dictionary<string, object> data)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo("ReturnRequestDoHeader", ""
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