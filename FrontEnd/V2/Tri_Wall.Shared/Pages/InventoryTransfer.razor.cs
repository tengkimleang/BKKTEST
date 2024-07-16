using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryTransfer;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.InventoryTransfer;

namespace Tri_Wall.Shared.Pages;

public partial class InventoryTransfer
{
    [Inject]
    public IValidator<InventoryTransferHeader>? Validator { get; init; }
    [Inject]
    public IValidator<InventoryTransferLine>? ValidatorLine { get; init; }

    private string stringDisplay = "Inventory Transfer";
    private string fromWord = "From";
    private string saveWord = "Save";
    string? dataGrid = "width: 1600px;height:405px";
    bool isView = false;
    protected void OnCloseOverlay() => visible = true;

    bool visible = false;

    async Task OpenDialogAsync(InventoryTransferLine deliveryOrderLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "warehouse", ViewModel.Warehouses },
            { "line", deliveryOrderLine },
            { "getSerialBatch", new Func<Dictionary<string,string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch) }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineInventoryTrasfer>(dictionary, new DialogParameters
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
            if (ViewModel.InventoryTransferForm.Lines == null) ViewModel.InventoryTransferForm.Lines = new List<InventoryTransferLine>();
            if (data["data"] is InventoryTransferLine receiptPoLine)
            {
                if (receiptPoLine.LineNum == 0)
                {
                    receiptPoLine.LineNum = ViewModel.InventoryTransferForm.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
                    ViewModel.InventoryTransferForm.Lines?.Add(receiptPoLine);
                }
                else
                {
                    var index = ViewModel.InventoryTransferForm.Lines.FindIndex(i => i.LineNum == receiptPoLine.LineNum);
                    ViewModel.InventoryTransferForm.Lines[index] = receiptPoLine;
                }
            }
        }
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
            stringDisplay = "Inventory Transfer";
            fromWord = "From";
            saveWord = "Save";
            dataGrid = "width: 1600px;height:405px";
        }
    }
    private void DeleteLine(int index)
    {
        ViewModel.InventoryTransferForm.Lines!.RemoveAt(index);
    }
    async Task OnSaveTransaction(string type = "")
    {
        ViewModel.InventoryTransferForm.DocDate = DateTime.Now;
        var result = await Validator!.ValidateAsync(ViewModel.InventoryTransferForm).ConfigureAwait(false);
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
                ViewModel.InventoryTransferForm = new InventoryTransferHeader();
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
            {"onSelete",new Func<string,Task>(OnSeleted)},
            {"onSearch",new Func<Dictionary<string,object>,Task<ObservableCollection<GetListData>>>(OnSearchGoodReceiptPo)},
            //{"onDelete",new Func<string,Task>(OnDelete)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Delivery Order",
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
            {"onSelete",new Func<string,Task>(OnSeletedPurchaseOrder)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Sales Order",
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
        ViewModel.InventoryTransferForm.DocDate = Convert.ToDateTime(objData?.DocDate);
        ViewModel.InventoryTransferForm.TaxDate = Convert.ToDateTime(objData?.TaxDate);
        await ViewModel.GetPurchaseOrderLineByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        ViewModel.InventoryTransferForm.Lines = new List<InventoryTransferLine>();
        var i = 1;
        foreach (var obj in ViewModel.GetPurchaseOrderLineByDocNums)
        {
            ViewModel.InventoryTransferForm.Lines?.Add(new InventoryTransferLine()
            {
                ItemCode = obj.ItemCode,
                ItemName = obj.ItemName,
                Qty = Convert.ToDouble(obj.Qty),
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
