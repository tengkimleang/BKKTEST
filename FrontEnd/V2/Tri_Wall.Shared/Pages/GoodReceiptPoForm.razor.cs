using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;

namespace Tri_Wall.Shared.Pages;

public partial class GoodReceiptPoForm
{
    [Inject]
    public IValidator<GoodReceiptPoHeader>? Validator { get; init; }
    // [Inject]
    // public IValidator<GoodReceiptPoLine>? ValidatorLine { init; get; }

    private string stringDisplay = "Good Receipt PO";
    private string fromWord = "From";
    private string saveWord = "Save";
    string? dataGrid = "width: 1600px;height:405px";
    bool isView;
    protected void OnCloseOverlay() => visible = true;

    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    bool visible;

    async Task OpenDialogAsync(MouseEventArgs e, GoodReceiptPoLine goodReceiptPoLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses },
            { "line", goodReceiptPoLine },
            { "getGenerateBatchSerial", new Func<Dictionary<string, object>, Task<string>>(OnGetGenerateBatchOrSerial) }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = string.IsNullOrEmpty(goodReceiptPoLine.ItemCode) ? "Add Line" : "Update Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);

        var result = await dialog.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is Dictionary<string, object> data)
        {
            if (ViewModel.GoodReceiptPoForm.Lines == null) ViewModel.GoodReceiptPoForm.Lines = new List<GoodReceiptPoLine>();
            if (data["data"] is GoodReceiptPoLine receiptPoLine)
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

    private async Task<string> OnGetGenerateBatchOrSerial(Dictionary<string, object> e)
    {
        await ViewModel.GetGennerateBatchSerialCommand.ExecuteAsync(e);
        return ViewModel.GetGenerateBatchSerial.FirstOrDefault()?.BatchOrSerial ?? "";
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
            stringDisplay = "Good Receipt PO";
            fromWord = "From";
            saveWord = "Save";
            dataGrid = "width: 1600px;height:405px";
        }
    }
    private void DeleteLine(MouseEventArgs e, int index)
    {
        ViewModel.GoodReceiptPoForm.Lines!.RemoveAt(index);
    }
    async Task OnSaveTransaction(MouseEventArgs e, string type = "")
    {
        await ErrorHandlingHelper.ExecuteWithHandlingAsync(async () =>
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
            visible = true;
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);
            Console.WriteLine(JsonSerializer.Serialize(ViewModel.PostResponses));
            if (ViewModel.PostResponses.ErrorCode == "")
            {
                selectedVendor = new List<Vendors>();
                ViewModel.GoodReceiptPoForm = new GoodReceiptPoHeader();
                ToastService.ShowSuccess("Success");
                if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry);
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
        }, ViewModel.PostResponses, ToastService).ConfigureAwait(false);
        visible = false;
    }
    Task OnSeleted(string e)
    {
        Console.WriteLine(e);
        ViewModel.GetGoodReceiptPoHeaderDeatialByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
        isView = true;
        StateHasChanged();
        return Task.CompletedTask;
    }
    // Task OnDelete(string e)
    // {
    //     Console.WriteLine(e);
    //     return Task.CompletedTask;
    // }
    Task OnView()
    {
        isView = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    async Task<ObservableCollection<GetListData>> OnSearchPurchaseOrder(Dictionary<string, object> e)
    {
        await ViewModel.GetPurchaseOrderBySearchCommand.ExecuteAsync(e);
        return ViewModel.GetListData;
    }

    async Task<ObservableCollection<GetListData>> OnSearchGoodReceiptPo(Dictionary<string, object> e)
    {
        await ViewModel.GetGoodReceiptPoBySearchCommand.ExecuteAsync(e);
        return ViewModel.GetListData;
    }

    async Task OnGetBatchOrSerial()
    {
        //Console.WriteLine(ViewModel.GetBatchOrSerials.Count());
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
        await ViewModel.TotalCountGoodReceiptPoCommand.ExecuteAsync(null).ConfigureAwait(false);
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
            Title = "List Good receipt PO",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }

    async Task ListCopyFromPurchaseOrder()
    {
        await ViewModel.TotalCountPurchaseOrderCommand.ExecuteAsync(null).ConfigureAwait(false);
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCountPurchaseOrder },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListDataPurchaseOrder) },
            //{ "isDelete", true },
            //{"onDelete",new Func<string,Task>(OnDelete)},
            { "isSelete", true },
            {"onSelete",new Func<string,Task>(OnSeletedPurchaseOrder)},
            {"onSearch",new Func<Dictionary<string,object>,Task<ObservableCollection<GetListData>>>(OnSearchPurchaseOrder)},
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
