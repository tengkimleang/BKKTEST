using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.VisualBasic;
using Refit;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.IssueForProduction;

namespace Tri_Wall.Shared.Pages;

public partial class InventoryCountingForm
{
    [Inject] public IValidator<IssueProductionHeader>? Validator { get; init; }
    [Inject] public IValidator<IssueProductionLine>? ValidatorLine { get; init; }

    private string stringDisplay = "Inventory Counting";
    private string saveWord = "Save";
    string? dataGrid = "width: 1600px;height:405px";
    bool isView = false;
    protected void OnCloseOverlay() => visible = true;
    private IEnumerable<GetProductionOrder> _getProductionOrder = new List<GetProductionOrder>();

    private IEnumerable<GetProductionOrder> SelectedProductionOrder
    {
        get => _getProductionOrder;
        set
        {
            if (value.Count() != 0)
            {
                string param = String.Empty;
                foreach (var obj in value)
                {
                    param = param + "''" + obj.DocEntry + "'',";
                }

                param = Strings.Left(param, Strings.Len(param) - 3);
                param += "''";
                ViewModel.GetPurchaseOrderLineByDocEntryCommand.ExecuteAsync(param).ConfigureAwait(false);
            }
            else
            {
                ViewModel.GetProductionOrderLines = new();
            }

            _getProductionOrder = value;
        }
    }

    bool visible;

    async Task<ObservableCollection<GetBatchOrSerial>> GetSerialBatch(Dictionary<string, string> dictionary)
    {
        await ViewModel.GetBatchOrSerialByItemCodeCommand.ExecuteAsync(dictionary);
        return ViewModel.GetBatchOrSerialsByItemCode;
    }

    async Task OpenDialogAsync(IssueProductionLine issueProductionLine)
    {
        Console.WriteLine(JsonSerializer.Serialize(ViewModel.GetProductionOrderLines));
        IEnumerable<GetProductionOrderLines> listGetProductionOrderLines = ViewModel.GetProductionOrderLines
            .GroupBy(item => new
            {
                item.ItemCode,
                item.ItemName,
                item.Uom,
                item.WarehouseCode,
                item.ItemType
            })
            .Select(group => new GetProductionOrderLines
            {
                ItemCode = group.Key.ItemCode,
                ItemName = group.Key.ItemName,
                Qty = (group.Sum(x => Convert.ToDouble(x.Qty))).ToString(CultureInfo.InvariantCulture),
                Uom = group.First().Uom,
                WarehouseCode = group.First().WarehouseCode,
                ItemType = group.First().ItemType,
            }).ToImmutableList();
        var dictionary = new Dictionary<string, object>
        {
            { "item", listGetProductionOrderLines },
            { "line", issueProductionLine },
            { "warehouse", ViewModel.Warehouses },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };
        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineIssueProductionOrder>(dictionary
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
            if (ViewModel.IssueProduction?.Lines == null)
                ViewModel.IssueProduction!.Lines = new List<IssueProductionLine>();
            if (data["data"] is IssueProductionLine issueProductionLineDialog)
            {
                if (issueProductionLineDialog.LineNum == 0)
                {
                    issueProductionLineDialog.LineNum =
                        ViewModel.IssueProductionLine?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
                    ViewModel.IssueProductionLine?.Add(issueProductionLineDialog);
                }
                else
                {
                    var index = ViewModel.IssueProductionLine.ToList()
                        .FindIndex(i => i.LineNum == issueProductionLineDialog.LineNum);
                    ViewModel.IssueProductionLine[index] = issueProductionLineDialog;
                }
            }
        }
    }

    private void OnSearch(OptionsSearchEventArgs<GetProductionOrder> e)
    {
        e.Items = ViewModel.GetProductionOrder.Where(i => i.DocNum.Contains(e.Text,
                                                              StringComparison.OrdinalIgnoreCase) ||
                                                          i.DocNum.Contains(e.Text
                                                              , StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.DocNum);
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
        ViewModel.IssueProductionLine!.RemoveAt(index);
    }

    async Task OnSaveTransaction(string type = "")
    {
        var issueProductionLines = ViewModel.IssueProductionLine.ToList();
        var serializedIssueProductionLine = JsonSerializer.Serialize(ViewModel.IssueProductionLine.AsQueryable());
        var serializedProductionOrderLines = JsonSerializer.Serialize(ViewModel.GetProductionOrderLines.AsQueryable());

        ViewModel.IssueProduction.Lines = new();

        foreach (var line in issueProductionLines)
        {
            var totalQty = ViewModel.GetProductionOrderLines?
                .Where(x => x.ItemCode == line.ItemCode)
                .Sum(x => Convert.ToDouble(x.Qty)) ?? 0;

            var productionOrderLines = ViewModel.GetProductionOrderLines!
                .Where(x => x.ItemCode == line.ItemCode)
                .ToList();

            foreach (var vmIssueProductionLine in productionOrderLines)
            {
                var actualQty = Math.Round((Convert.ToDouble(vmIssueProductionLine.Qty ?? "0") / totalQty) * line.Qty, 6);

                if (line.ManageItem == "S")
                {
                    ProcessSerials(line, vmIssueProductionLine, actualQty);
                }
                else if (line.ManageItem == "B")
                {
                    ProcessBatches(line, vmIssueProductionLine, actualQty, issueProductionLines);
                }
                else
                {
                    AddIssueProductionLine(vmIssueProductionLine, line, actualQty);
                }
            }
        }

        RestoreViewModelState(serializedIssueProductionLine, serializedProductionOrderLines);

        Console.WriteLine(JsonSerializer.Serialize(ViewModel.IssueProduction));
        var result = await Validator!.ValidateAsync(ViewModel.IssueProduction).ConfigureAwait(false);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }
            return;
        }

        await SubmitTransaction(type);
    }

    private void ProcessSerials(IssueProductionLine line, GetProductionOrderLines vmIssueProductionLine, double actualQty)
    {
        var serials = new List<SerialIssueProduction>();

        for (var i = 0; i < actualQty; i++)
        {
            if (line.Serials?.Count > 0)
            {
                serials.Add(line.Serials.First());
                line.Serials.RemoveAt(0);
            }
        }

        ViewModel.IssueProduction.Lines.Add(new IssueProductionLine
        {
            ItemCode = vmIssueProductionLine.ItemCode,
            ItemName = vmIssueProductionLine.ItemName,
            Qty = actualQty,
            UomName = vmIssueProductionLine.Uom,
            WhsCode = line.WhsCode,
            ManageItem = vmIssueProductionLine.ItemType,
            BaseLineNum = Convert.ToInt32(vmIssueProductionLine.OrderLineNum),
            DocNum = vmIssueProductionLine.DocEntry,
            Serials = serials,
        });
    }

    private void ProcessBatches(IssueProductionLine line, GetProductionOrderLines vmIssueProductionLine, double actualQty, List<IssueProductionLine> issueProductionLines)
    {
        var batches = new List<BatchIssueProduction>();
        var remainingQty = actualQty;

        foreach (var batch in line.Batches!.ToList())
        {
            if (remainingQty == 0) break;

            var index = line.Batches!.IndexOf(batch);
            var indexHeader = issueProductionLines.IndexOf(line);

            batches.Add(new BatchIssueProduction
            {
                AdmissionDate = batch.AdmissionDate,
                BatchCode = batch.BatchCode ?? "",
                ExpDate = batch.ExpDate,
                Qty = Math.Min(batch.Qty, remainingQty)
            });

            if (batch.Qty > remainingQty)
            {
                issueProductionLines[indexHeader].Batches![index].Qty = Math.Round(batch.Qty - remainingQty, 6);
                remainingQty = 0;
            }
            else
            {
                remainingQty -= batch.Qty;
                issueProductionLines[indexHeader].Batches![index].Qty = 0;
            }

            if (batch.Qty == 0)
            {
                line.Batches!.Remove(batch);
            }
        }

        if (batches.Count > 0)
        {
            ViewModel.IssueProduction.Lines.Add(new IssueProductionLine
            {
                ItemCode = vmIssueProductionLine.ItemCode,
                ItemName = vmIssueProductionLine.ItemName,
                Qty = actualQty,
                UomName = vmIssueProductionLine.Uom,
                WhsCode = line.WhsCode,
                ManageItem = vmIssueProductionLine.ItemType,
                BaseLineNum = Convert.ToInt32(vmIssueProductionLine.OrderLineNum),
                DocNum = vmIssueProductionLine.DocEntry,
                Batches = batches,
            });
        }
    }

    private void AddIssueProductionLine(GetProductionOrderLines vmIssueProductionLine, IssueProductionLine line, double actualQty)
    {
        ViewModel.IssueProduction.Lines.Add(new IssueProductionLine
        {
            ItemCode = vmIssueProductionLine.ItemCode,
            ItemName = vmIssueProductionLine.ItemName,
            Qty = actualQty,
            UomName = vmIssueProductionLine.Uom,
            WhsCode = line.WhsCode,
            ManageItem = vmIssueProductionLine.ItemType,
            BaseLineNum = Convert.ToInt32(vmIssueProductionLine.OrderLineNum),
            DocNum = vmIssueProductionLine.DocEntry
        });
    }

    private void RestoreViewModelState(string serializedIssueProductionLine, string serializedProductionOrderLines)
    {
        ViewModel.IssueProductionLine = JsonSerializer.Deserialize<ObservableCollection<IssueProductionLine>>(serializedIssueProductionLine) ?? new();
        ViewModel.GetProductionOrderLines = JsonSerializer.Deserialize<ObservableCollection<GetProductionOrderLines>>(serializedProductionOrderLines) ?? new();
    }

    private async Task SubmitTransaction(string type)
    {
        try
        {
            visible = true;

            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (string.IsNullOrEmpty(ViewModel.PostResponses.ErrorCode))
            {
                SelectedProductionOrder = new List<GetProductionOrder>();
                ViewModel.IssueProduction = new();
                ViewModel.IssueProductionLine = new();
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
            {"onSearch",new Func<Dictionary<string,object>,Task<ObservableCollection<GetListData>>>(OnSearchGoodReceiptPo)},
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