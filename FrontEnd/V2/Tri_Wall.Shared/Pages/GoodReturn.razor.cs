﻿using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.GoodReturn;
using Tri_Wall.Shared.Views.Return;

namespace Tri_Wall.Shared.Pages;

public partial class GoodReturn
{
    [Inject] public IValidator<DeliveryOrderHeader>? Validator { get; init; }
    [Inject] public IValidator<DeliveryOrderLine>? ValidatorLine { get; init; }

    private string stringDisplay = "Good Return";
    private string fromWord = "From";
    private string saveWord = "Save";
    string? dataGrid = "width: 1600px;height:405px";
    bool isView = false;
    protected void OnCloseOverlay() => visible = true;

    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    bool visible = false;

    async Task OpenDialogAsync(DeliveryOrderLine deliveryOrderLine)
    {
        Console.WriteLine(JsonSerializer.Serialize(deliveryOrderLine));
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses },
            { "line", deliveryOrderLine },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineGoodReturn>(dictionary, new DialogParameters
        {
            Title = (deliveryOrderLine == null) ? "Add Line" : "Update Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);

        var result = await dialog.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is Dictionary<string, object> data)
        {
            if (ViewModel.DeliveryOrderForm.Lines == null)
                ViewModel.DeliveryOrderForm.Lines = new List<DeliveryOrderLine>();
            if (data["data"] is DeliveryOrderLine receiptPoLine)
            {
                if (receiptPoLine.LineNum == 0)
                {
                    receiptPoLine.LineNum = ViewModel.DeliveryOrderForm.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
                    ViewModel.DeliveryOrderForm.Lines?.Add(receiptPoLine);
                }
                else
                {
                    var index = ViewModel.DeliveryOrderForm.Lines.FindIndex(i => i.LineNum == receiptPoLine.LineNum);
                    ViewModel.DeliveryOrderForm.Lines[index] = receiptPoLine;
                }
            }
        }
    }

    private void OnSearch(OptionsSearchEventArgs<Vendors> e)
    {
        e.Items = ViewModel.Vendors.Where(i => i.VendorCode.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                                                 i.VendorName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.VendorCode);
    }

    void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            stringDisplay = "";
            dataGrid = "width: 1600px;height:205px";
            fromWord = "";
            saveWord = "S-";
        }
        else
        {
            stringDisplay = "Good Return";
            fromWord = "From";
            saveWord = "Save";
            dataGrid = "width: 1600px;height:405px";
        }
    }

    private void DeleteLine(int index)
    {
        ViewModel.DeliveryOrderForm.Lines!.RemoveAt(index);
    }

    async Task OnSaveTransaction(string type = "")
    {
        ViewModel.DeliveryOrderForm.CustomerCode = selectedVendor.FirstOrDefault()?.VendorCode ?? "";
        ViewModel.DeliveryOrderForm.DocDate = DateTime.Now;
        var result = await Validator!.ValidateAsync(ViewModel.DeliveryOrderForm).ConfigureAwait(false);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }

            return;
        }

        try
        {
            visible = true;
            Console.WriteLine(JsonSerializer.Serialize(ViewModel.DeliveryOrderForm));
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (ViewModel.PostResponses.ErrorCode == "")
            {
                selectedVendor = new List<Vendors>();
                ViewModel.DeliveryOrderForm = new DeliveryOrderHeader();
                ToastService.ShowSuccess("Success");
                if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);

            visible = false;
        }
        catch (ApiException ex)
        {
            var content = ex.GetContentAsAsync<Dictionary<String, String>>();
            ToastService!.ShowError(ex.ReasonPhrase ?? "");
            visible = false;
        }
    }

    Task OnSeleted(string e)
    {
        Console.WriteLine(e);
        ViewModel.GetGoodReceiptPoHeaderDeatialByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        isView = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    Task OnDelete(string e)
    {
        Console.WriteLine(e);
        return Task.CompletedTask;
    }

    Task OnView()
    {
        isView = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    async Task OnGetBatchOrSerial()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "getData", ViewModel.GetBatchOrSerials },
        };
        await DialogService!.ShowDialogAsync<ListBatchOrSerial>(dictionary, new DialogParameters
        {
            Title = "List Batch Or Serial",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }

    async Task<ObservableCollection<GetListData>> GetListData(int p)
    {
        //OnGetPurchaseOrder
        await ViewModel.GetGoodReceiptPoCommand.ExecuteAsync(p.ToString());
        return ViewModel.GetListData;
    }

    async Task<ObservableCollection<GetListData>> OnSearchGoodReceiptPo(Dictionary<string, object> e)
    {
        await ViewModel.GetGoodReceiptPoBySearchCommand.ExecuteAsync(e);
        return ViewModel.GetListData;
    }

    async Task OpenListDataAsyncAsync()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCount },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListData) },
            //{ "isDelete", true },
            { "isSelete", true },
            { "onSelete", new Func<string, Task>(OnSeleted) },
            {
                "onSearch",
                new Func<Dictionary<string, object>, Task<ObservableCollection<GetListData>>>(OnSearchGoodReceiptPo)
            },
            //{"onDelete",new Func<string,Task>(OnDelete)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Good Reutrn",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }

    async Task<ObservableCollection<GetBatchOrSerial>> GetSerialBatch(Dictionary<string, string> dictionary)
    {
        await ViewModel.GetBatchOrSerialByItemCodeCommand.ExecuteAsync(dictionary);
        return ViewModel.GetBatchOrSerialsByItemCode;
    }

    async Task ListCopyFromPurchaseOrder()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCountSalesOrder },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListDataPurchaseOrder) },
            //{ "isDelete", true },
            //{"onDelete",new Func<string,Task>(OnDelete)},
            { "isSelete", true },
            { "onSelete", new Func<string, Task>(OnSeletedPurchaseOrder) },
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Good Receipt PO",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }

    async Task<ObservableCollection<GetListData>> GetListDataPurchaseOrder(int p)
    {
        await ViewModel.GetPurchaseOrderCommand.ExecuteAsync(p.ToString());
        return ViewModel.GetListData;
    }

    async Task OnSeletedPurchaseOrder(string e)
    {
        Console.WriteLine(e);
        var objData = ViewModel.GetListData.FirstOrDefault(x => x.DocEntry.ToString() == e);
        ViewModel.DeliveryOrderForm.DocDate = Convert.ToDateTime(objData?.DocDate);
        ViewModel.DeliveryOrderForm.TaxDate = Convert.ToDateTime(objData?.TaxDate);
        selectedVendor = ViewModel.Vendors.Where(x => x.VendorCode == objData?.VendorCode);
        await ViewModel.GetPurchaseOrderLineByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        ViewModel.DeliveryOrderForm.Lines = new List<DeliveryOrderLine>();
        var i = 1;
        foreach (var obj in ViewModel.GetPurchaseOrderLineByDocNums)
        {
            var batch = new List<BatchDeliveryOrder>();
            var serial = new List<SerialDeliveryOrder>();
            foreach (var objBatch in obj.Batches)
            {
                batch.Add(new BatchDeliveryOrder
                {
                    AdmissionDate = objBatch.AdmissionDate,
                    BatchCode = objBatch.BatchCode,
                    ExpDate = objBatch.ExpDate,
                    LotNo = objBatch.LotNo,
                    ManfectureDate = objBatch.ManfectureDate,
                    Qty = objBatch.Qty,
                    QtyAvailable = objBatch.QtyAvailable,
                    OnSelectedBatchOrSerial = objBatch.OnSelectedBatchOrSerial,
                });
            }

            foreach (var objSerial in obj.Serials)
            {
                serial.Add(new SerialDeliveryOrder
                {
                    ExpDate = objSerial.ExpDate,
                    MfrDate = objSerial.MfrDate,
                    Qty = objSerial.Qty,
                    SerialCode = objSerial.SerialCode,
                    MfrNo = objSerial.MfrNo,
                    OnSelectedBatchOrSerial = objSerial.OnSelectedBatchOrSerial,
                });
            }

            ViewModel.DeliveryOrderForm.Lines?.Add(new DeliveryOrderLine()
            {
                ItemCode = obj.ItemCode,
                ItemName = obj.ItemName,
                Qty = Convert.ToDouble(obj.Qty),
                Price = Convert.ToDouble(obj.Price),
                VatCode = obj.VatCode,
                WarehouseCode = obj.WarehouseCode,
                ManageItem = obj.ManageItem,
                LineNum = i,
                BaseEntry = Convert.ToInt32(obj.DocEntry),
                BaseLine = Convert.ToInt32(obj.BaseLineNumber),
                Batches = batch,
                Serials = serial
            });
            i++;
        }

        StateHasChanged();
    }
}