﻿using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Views.GoodReceptPo;

namespace Tri_Wall.Shared.Pages;

public partial class GoodReceiptPoForm
{
    [Inject]
    public IValidator<GoodReceiptPoHeader>? Validator { get; init; }
    [Inject]
    public IValidator<GoodReceiptPoLine>? ValidatorLine { get; init; }
    
    private string stringDisplay = "Recept";
    
    string? dataGrid = "width: 100%;height:405px";
    bool isView=false;
    protected void OnCloseOverlay() => visible = true;
    
    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    bool visible = false;
    
    async Task OpenDialogAsync(GoodReceiptPoLine goodReceiptPoLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses },
            { "line", goodReceiptPoLine }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = (goodReceiptPoLine == null) ? "Add Line" : "Update Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);

        var result = await dialog.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is Dictionary<string, object> data)
        {
            if(ViewModel.GoodReceiptPoForm.Lines==null) ViewModel.GoodReceiptPoForm.Lines = new List<GoodReceiptPoLine>();
            if(data["data"] is GoodReceiptPoLine receiptPoLine)
            {
                if (receiptPoLine.LineNum == 0)
                {
                    receiptPoLine.LineNum = ViewModel.GoodReceiptPoForm.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
                    ViewModel.GoodReceiptPoForm.Lines?.Add(receiptPoLine);
                }
                else
                {
                    var index = ViewModel.GoodReceiptPoForm.Lines.FindIndex(i => i.LineNum == receiptPoLine.LineNum);
                    ViewModel.GoodReceiptPoForm.Lines[index] = receiptPoLine;
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
            dataGrid = "width: 700px;height:205px";
        }
        else
        {
            stringDisplay = "Recept";
            dataGrid = "width: 100%;height:405px";
        }
    }
    private void DeleteLine(int index)
    {
        ViewModel.GoodReceiptPoForm.Lines!.RemoveAt(index);
    }
    async Task OnSaveTransaction(string type="")
    {
        ViewModel.GoodReceiptPoForm.VendorCode = selectedVendor.FirstOrDefault()?.VendorCode ?? "";
        ViewModel.GoodReceiptPoForm.DocDate = DateTime.Now;
        var result = await Validator!.ValidateAsync(ViewModel.GoodReceiptPoForm).ConfigureAwait(false);
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
            
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (ViewModel.PostResponses.ErrorCode == "")
            {
                selectedVendor=new List<Vendors>();
                ViewModel.GoodReceiptPoForm= new GoodReceiptPoHeader();
                ToastService.ShowSuccess("Success");
                if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
            visible = false;
        }
        catch (Exception ex)
        {
            string errorMessage = ex.InnerException?.Message ?? ex.Message;
            ToastService!.ShowError(errorMessage);
            visible = false;
        }
    }
    Task OnSeleted(string e)
    {
        Console.WriteLine(e);
        ViewModel.GetGoodReceiptPoHeaderDeatialByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        isView =true;
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
        Console.WriteLine(ViewModel.GetBatchOrSerials.Count());
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
    async Task OpenListDataAsyncAsync()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCount },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListData) },
            //{ "isDelete", true },
            { "isSelete", true },
            {"onSelete",new Func<string,Task>(OnSeleted)},
            //{"onDelete",new Func<string,Task>(OnDelete)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Good receipt PO",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }

    async Task ListCopyFromPurchaseOrder()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCountPurchaseOrder },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListDataPurchaseOrder) },
            //{ "isDelete", true },
            //{"onDelete",new Func<string,Task>(OnDelete)},
            { "isSelete", true },
            {"onSelete",new Func<string,Task>(OnSeletedPurchaseOrder)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Purchase Order",
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
        ViewModel.GoodReceiptPoForm.DocDate = Convert.ToDateTime(objData?.DocDate);
        ViewModel.GoodReceiptPoForm.TaxDate = Convert.ToDateTime(objData?.TaxDate);
        selectedVendor = ViewModel.Vendors.Where(x => x.VendorCode == objData?.VendorCode);
        await ViewModel.GetPurchaseOrderLineByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        ViewModel.GoodReceiptPoForm.Lines = new List<GoodReceiptPoLine>();
        var i = 1;
        foreach (var obj in ViewModel.GetPurchaseOrderLineByDocNums)
        {
            ViewModel.GoodReceiptPoForm.Lines?.Add(new GoodReceiptPoLine
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
            });
            i++;
        }
        StateHasChanged();
    }
}
