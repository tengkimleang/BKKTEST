using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryCounting;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.InventoryCounting;

namespace Tri_Wall.Shared.Pages;

public partial class InventoryCountingForm
{
    [Inject] public IValidator<InventoryCountingHeader>? Validator { get; init; }
    [Inject] public IValidator<InventoryCountingLine>? ValidatorLine { get; init; }
    [Inject] public Blazored.LocalStorage.ISyncLocalStorageService? LocalStorage { get; init; }

    private string stringDisplay = "Inventory Counting";
    private string saveWord = "Save";
    string? dataGrid = "width: 1600px;height:405px";
    bool isView = false;
    protected void OnCloseOverlay() => visible = true;
    private IEnumerable<GetInventoryCountingList> _getProductionOrder = new List<GetInventoryCountingList>();

    private IEnumerable<GetInventoryCountingList> SelectedProductionOrder
    {
        get => _getProductionOrder;
        set
        {
            if (!value.Any()) return;
            ViewModel.InventoryCountingHeader.DocEntry = Convert.ToInt32(value.ToList()[0].DocEntry);
            ViewModel.InventoryCountingHeader.Series = value.ToList()[0].Series;
            ViewModel.InventoryCountingHeader.CreateDate = Convert.ToDateTime(value.ToList()[0].CreateDate);
            ViewModel.InventoryCountingHeader.CreateTime = value.ToList()[0].CreateTime;
            ViewModel.InventoryCountingHeader.OtherRemark = value.ToList()[0].OtherRemark;
            ViewModel.InventoryCountingHeader.Ref2 = value.ToList()[0].Ref2;
            ViewModel.InventoryCountingHeader.InventoryCountingType=value.ToList()[0].InventoryCountingType;
            Console.WriteLine(value.ToList()[0].DocEntry);
            ViewModel.GetPurchaseOrderLineByDocEntryCommand.ExecuteAsync(value.ToList()[0].DocEntry)
                .ConfigureAwait(false);
            _getProductionOrder = value;
            StateHasChanged();
        }
    }

    bool visible;

    async Task<ObservableCollection<GetBatchOrSerial>> GetSerialBatch(Dictionary<string, string> dictionary)
    {
        Console.WriteLine(JsonSerializer.Serialize(dictionary));
        await ViewModel.GetBatchOrSerialByItemCodeCommand.ExecuteAsync(dictionary);
        return ViewModel.GetBatchOrSerialsByItemCode;
    }

    async Task OpenDialogAsync(InventoryCountingLine issueProductionLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.GetInventoryCountingLines },
            { "line", issueProductionLine },
            { "warehouse", ViewModel.Warehouses },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };
        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineInventoryCounting>(dictionary
            , new DialogParameters
            {
                Title = (issueProductionLine.ItemCode == "") ? "Add Line" : "Update Line",
                PreventDismissOnOverlayClick = true,
                PreventScroll = false,
                Width = "80%",
                Height = "80%"
            }).ConfigureAwait(false);

        var result = await dialog.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is Dictionary<string, object> data)
        {
            Console.WriteLine(JsonSerializer.Serialize(result.Data));
            if (ViewModel.InventoryCountingHeader?.Lines == null)
                ViewModel.InventoryCountingHeader!.Lines = new List<InventoryCountingLine>();
            if (data["data"] is InventoryCountingLine inventoryCountingLineDialog)
            {
                Console.WriteLine(JsonSerializer.Serialize(inventoryCountingLineDialog));
                if (inventoryCountingLineDialog.LineNum == 0)
                {
                    inventoryCountingLineDialog.LineNum =
                        ViewModel.InventoryCountingHeader?.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
                    ViewModel.InventoryCountingHeader?.Lines?.Add(inventoryCountingLineDialog);
                }
                else
                {
                    var index = ViewModel.InventoryCountingHeader.Lines.ToList()
                        .FindIndex(i => i.LineNum == inventoryCountingLineDialog.LineNum);
                    ViewModel.InventoryCountingHeader.Lines[index] = inventoryCountingLineDialog;
                }
            }
        }
    }

    private void OnSearch(OptionsSearchEventArgs<GetInventoryCountingList> e)
    {
        e.Items = ViewModel.GetInventoryCountingLists.Where(i => i.Series.Contains(e.Text,
                                                                     StringComparison.OrdinalIgnoreCase) ||
                                                                 i.Series.Contains(e.Text
                                                                     , StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.Series);
    }

    void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            stringDisplay = "";
            dataGrid = "width: 1600px;height:205px";
            saveWord = "S-";
        }
        else
        {
            stringDisplay = "Inventory Counting";
            saveWord = "Save";
            dataGrid = "width: 1600px;height:405px";
        }
    }

    private void DeleteLine(int index)
    {
        ViewModel.InventoryCountingHeader.Lines!.RemoveAt(index);
    }

    async Task OnSaveTransaction(string type = "")
    {
        await ErrorHandlingHelper.ExecuteWithHandlingAsync(async () =>
        {
            var result = await Validator!.ValidateAsync(ViewModel.InventoryCountingHeader).ConfigureAwait(false);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ToastService!.ShowError(error.ErrorMessage);
                }
                return;
            }
            visible = true;
            await SubmitTransaction(type);
        }, ViewModel.PostResponses, ToastService).ConfigureAwait(false);
        visible = false;        
    }

    private void AddIssueProductionLine(GetProductionOrderLines vmIssueProductionLine, IssueProductionLine line,
        double actualQty)
    {
        ViewModel.InventoryCountingHeader.Lines.Add(new InventoryCountingLine
        {
            ItemCode = vmIssueProductionLine.ItemCode,
            // ItemName = vmIssueProductionLine.ItemName,
            Qty = actualQty,
            // UomName = vmIssueProductionLine.Uom,
            // WhsCode = line.WhsCode,
            ManageItem = vmIssueProductionLine.ItemType,
            // BaseLineNum = Convert.ToInt32(vmIssueProductionLine.OrderLineNum),
            // DocNum = vmIssueProductionLine.DocEntry
        });
    }

    private async Task SubmitTransaction(string type)
    {
        try
        {
            visible = true;

            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (string.IsNullOrEmpty(ViewModel.PostResponses.ErrorCode))
            {
                SelectedProductionOrder = new List<GetInventoryCountingList>();
                ViewModel.InventoryCountingHeader = new();
                ViewModel.InventoryCountingLines = new();
                ToastService.ShowSuccess("Success");

                if (type == "print")
                {
                    await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
                }
            }
            else
            {
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
            }

            visible = false;
        }
        catch (ApiException ex)
        {
            var content = await ex.GetContentAsAsync<Dictionary<string, string>>();
            ToastService!.ShowError(ex.ReasonPhrase ?? "");
            visible = false;
        }
    }

    Task OnSeleted(string e)
    {
        // Console.WriteLine(e);
        ViewModel.IssueForProductionDeatialByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
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

    async Task<ObservableCollection<GetListData>> GetListData(int p)
    {
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
            Title = "List Issue For Production",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
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
}