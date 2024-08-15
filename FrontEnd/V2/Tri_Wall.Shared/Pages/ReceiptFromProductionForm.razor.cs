using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.VisualBasic;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.ReturnComponentProduction;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.ReceiptFromProduction;

namespace Tri_Wall.Shared.Pages;

public partial class ReceiptFromProductionForm
{
    [Inject] public IValidator<ReturnComponentProductionHeader>? Validator { get; init; }
    [Inject] public IValidator<ReturnComponentProductionLine>? ValidatorLine { get; init; }

    private string stringDisplay = "Receipt From Production";
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

    async Task OpenDialogAsync(ReturnComponentProductionLine issueProductionLine)
    {
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
                DocEntry = group.First().DocEntry,
                PlanQty = group.Sum(x => Convert.ToDouble(x.PlanQty)).ToString(CultureInfo.InvariantCulture),
            }).ToImmutableList();

        var dictionary = new Dictionary<string, object>
        {
            { "item", listGetProductionOrderLines },
            { "line", issueProductionLine },
            { "warehouse", ViewModel.Warehouses },
            { "docNumOrderSelected", SelectedProductionOrder },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };
        var dialog = await DialogService!.ShowDialogAsync<DialogAddLineReceiptFromProductionOrder>(dictionary
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
            if (ViewModel.ReceiptFromProductionOrderForm?.Lines == null)
                ViewModel.ReceiptFromProductionOrderForm!.Lines = new List<ReturnComponentProductionLine>();
            if (data["data"] is ReturnComponentProductionLine issueProductionLineDialog)
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
            stringDisplay = "Receipt For Production";
            saveWord = "Save";
            dataGrid = "width: 1600px;height:405px";
        }
    }

    private void DeleteLine(int index)
    {
        ViewModel.ReceiptFromProductionOrderForm.Lines!.RemoveAt(index);
    }

    async Task OnSaveTransaction(string type = "")
    {
        var productionOrder = ViewModel.IssueProductionLine.ToList();
        ViewModel.ReceiptFromProductionOrderForm.Lines = new();
        var strMP = JsonSerializer.Serialize(ViewModel.IssueProductionLine.AsQueryable());
        foreach (var line in productionOrder.ToList())
        {
            if (line.ManageItem == "N")
            {
                ProcessItemNones(line);
            }
            else if (line.ManageItem == "B")
            {
                ProcessItemBatch(line);
            }
            else if (line.ManageItem == "S")
            {
                ProcessItemSerial(line);
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(ViewModel.ReceiptFromProductionOrderForm));
        ViewModel.IssueProductionLine = JsonSerializer.Deserialize<ObservableCollection<ReturnComponentProductionLine>>(strMP) ?? new();
    }

    Task OnSeleted(string e)
    {
        ViewModel.IssueForProductionDetailByDocNumCommand.ExecuteAsync(e).ConfigureAwait(false);
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
    #region OnSaveTransaction Logic
    void ProcessItemNones(ReturnComponentProductionLine line)
    {
        if (line.ItemNones != null)
        {
            foreach (var lineManual in line.ItemNones)
            {
                var totalManualQty = ViewModel.GetProductionOrderLines!.Where(x =>
                            x.ItemCode == line.ItemCode
                            && x.DocEntry == lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry
                            )
                            .Sum(x => Convert.ToDouble(x.Qty));
                foreach (var AddLineManual in ViewModel.GetProductionOrderLines!.Where(x =>
                    x.ItemCode == line.ItemCode
                    && x.DocEntry == lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry))
                {
                    ViewModel.ReceiptFromProductionOrderForm.Lines.Add(new ReturnComponentProductionLine
                    {
                        DocNum = AddLineManual.DocEntry,
                        BaseLineNum = Convert.ToInt32(AddLineManual.OrderLineNum ?? "0"),
                        ItemCode = line.ItemCode,
                        ItemName = line.ItemName,
                        Qty = (Convert.ToDouble(AddLineManual.Qty ?? "0") / totalManualQty) * lineManual.Qty,
                        QtyRequire = line.QtyRequire,
                        QtyPlan = line.QtyPlan,
                        QtyManual = lineManual.Qty,
                        QtyLost = (Convert.ToDouble(AddLineManual.Qty ?? "0") / totalManualQty) * lineManual.QtyLost,
                        Price = line.Price,
                        WhsCode = line.WhsCode,
                        UomName = "Manual None",
                    });
                }
            }

            var total = ViewModel.GetProductionOrderLines!.Where(x =>
                            x.ItemCode == line.ItemCode
                            && !ViewModel.ReceiptFromProductionOrderForm.Lines.Where(z => z.Qty > 0).Select(x => x.DocNum).Contains(x.DocEntry)
                            )
                            .Sum(x => Convert.ToDouble(x.Qty));
            var tmp = new List<ReturnComponentProductionLine>();
            foreach (var LineAuto in ViewModel.GetProductionOrderLines!.Where(x =>
                        x.ItemCode == line.ItemCode
                        && !ViewModel.ReceiptFromProductionOrderForm.Lines.Where(z => z.Qty > 0)
                            .Select(x => x.DocNum).Contains(x.DocEntry)
                        ))
            {
                tmp.Add(new ReturnComponentProductionLine
                {
                    DocNum = LineAuto.DocEntry,
                    LineNum = line.LineNum,
                    BaseLineNum = Convert.ToInt32(LineAuto.OrderLineNum ?? "0"),
                    ItemCode = line.ItemCode,
                    ItemName = line.ItemName,
                    Qty = (Convert.ToDouble(LineAuto.Qty ?? "0") / total) * line.Qty,
                    QtyRequire = line.QtyRequire,
                    QtyPlan = line.QtyPlan,
                    QtyManual = 0,
                    QtyLost = (Convert.ToDouble(LineAuto.Qty ?? "0") / total) * line.QtyLost,
                    Price = line.Price,
                    WhsCode = line.WhsCode,
                    UomName = "Auto None",
                });
            }
            ViewModel.ReceiptFromProductionOrderForm.Lines.AddRange(tmp);
        }
    }
    void ProcessItemBatch(ReturnComponentProductionLine line)
    {
        if (line.Batches != null)
        {
            var tmpManual = new List<ReturnComponentProductionLine>();

            foreach (var lineManual in line.Batches)
            {
                var selectedProductionOrderDocEntry = lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry;
                var matchingProductionOrderLines = ViewModel.GetProductionOrderLines!
                    .Where(x => x.ItemCode == line.ItemCode && x.DocEntry == selectedProductionOrderDocEntry)
                    .ToList(); // Perform the filtering once and reuse the result.

                if (!matchingProductionOrderLines.Any()) continue; // Skip if no matching lines found.

                var totalManualQty = matchingProductionOrderLines
                    .Sum(x => Convert.ToDouble(x.Qty));

                foreach (var addLineManual in matchingProductionOrderLines)
                {
                    var manualLine = new ReturnComponentProductionLine
                    {
                        DocNum = addLineManual.DocEntry,
                        BaseLineNum = Convert.ToInt32(addLineManual.OrderLineNum ?? "0"),
                        ItemCode = line.ItemCode,
                        ItemName = line.ItemName,
                        Qty = Math.Round((Convert.ToDouble(addLineManual.Qty ?? "0") / totalManualQty) * lineManual.Qty, 6),
                        QtyRequire = line.QtyRequire,
                        QtyPlan = line.QtyPlan,
                        QtyManual = lineManual.Qty,
                        QtyLost = Math.Round(
                            (Convert.ToDouble(addLineManual.Qty ?? "0") / totalManualQty) * lineManual.QtyLost, 6),
                        Price = line.Price,
                        WhsCode = line.WhsCode,
                        UomName = "Manual Batch",
                        Type = 2,
                    };
                    tmpManual.Add(manualLine);
                }
            }

            ViewModel.ReceiptFromProductionOrderForm.Lines.AddRange(tmpManual); // Add outside the loop to avoid repeated additions.

            // Calculate total quantity considering all production order lines related to the item code.
            var totalQty = ViewModel.GetProductionOrderLines!
                .Where(x => x.ItemCode == line.ItemCode)
                .Sum(x => Convert.ToDouble(x.Qty));

            // Calculate total quantity for auto lost by filtering production order lines not referenced in IssueProduction.Lines or where QtyLost is 0.
            var referencedDocEntries = ViewModel.ReceiptFromProductionOrderForm.Lines
                .Where(z => z.QtyLost == 0)
                .Select(z => z.DocNum)
                .Distinct();

            var totalQtyAutoLost = ViewModel.GetProductionOrderLines!
                .Where(x => x.ItemCode == line.ItemCode && !referencedDocEntries.Contains(x.DocEntry))
                .Sum(x => Convert.ToDouble(x.Qty));

            // Initialize a new list for ReturnComponentProductionLine objects.
            var tmp = new List<ReturnComponentProductionLine>();
            foreach (var lineAuto in ViewModel.GetProductionOrderLines.Where(x => x.ItemCode == line.ItemCode))
            {
                bool existsInIssueProduction = ViewModel.ReceiptFromProductionOrderForm.Lines.Any(x =>
                    x.DocNum == lineAuto.DocEntry && x.BaseLineNum == Convert.ToInt32(lineAuto.OrderLineNum ?? "0"));

                if (!existsInIssueProduction)
                {
                    double qty = Convert.ToDouble(lineAuto.Qty ?? "0");
                    double calculatedQty = Math.Round((qty / totalQty) * line.Qty);
                    double calculatedQtyLost = Math.Round((qty / totalQtyAutoLost) * line.QtyLost);

                    tmp.Add(new ReturnComponentProductionLine
                    {
                        DocNum = lineAuto.DocEntry,
                        LineNum = line.LineNum,
                        BaseLineNum = Convert.ToInt32(lineAuto.OrderLineNum ?? "0"),
                        ItemCode = line.ItemCode,
                        ItemName = line.ItemName,
                        Qty = calculatedQty,
                        QtyRequire = line.QtyRequire,
                        QtyPlan = line.QtyPlan,
                        QtyManual = 0,
                        QtyLost = calculatedQtyLost,
                        Price = line.Price,
                        WhsCode = line.WhsCode,
                        UomName = "Auto Batch",
                        Type = 1,
                    });
                }
                else
                {
                    foreach (var issueLine in ViewModel.ReceiptFromProductionOrderForm.Lines.Where(x =>
                                 x.DocNum == lineAuto.DocEntry && x.Qty == 0 &&
                                 x.BaseLineNum == Convert.ToInt32(lineAuto.OrderLineNum ?? "0")))
                    {
                        double qty = Convert.ToDouble(lineAuto.Qty ?? "0");
                        issueLine.Qty = Math.Round((qty / totalQty) * line.Qty);
                    }
                }
            }

            ViewModel.ReceiptFromProductionOrderForm.Lines.AddRange(tmp);
            totalQtyAutoLost = line.Batches
                .Sum(x => x.OnSelectedType.Any(type => type.Id == 1) ? x.Qty : 0);
            tmp = new List<ReturnComponentProductionLine>();
            ViewModel.ReceiptFromProductionOrderForm.Lines.ToList().ForEach(x =>
            {
                if (x.Type == 2)
                {
                    foreach (var z in line.Batches.ToList().Where(all =>
                                 all.OnSelectedType.FirstOrDefault()?.Id == 2
                                 && all.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry == x.DocNum
                             ).ToList())
                    {
                        var batches = new List<BatchReturnComponentProduction>();
                        var totalManualQty = ViewModel.GetProductionOrderLines!.Where(all =>
                                all.ItemCode == line.ItemCode
                                && all.DocEntry == z.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry)
                            .Sum(all => Convert.ToDouble(all.Qty));
                        var qty = Convert.ToDouble(ViewModel.GetProductionOrderLines!.FirstOrDefault(all =>
                            all.ItemCode == line.ItemCode
                            && all.DocEntry == z.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry
                            && all.OrderLineNum == x.BaseLineNum.ToString())?.Qty ?? "0");
                        var remainingQty = Math.Round(qty / totalManualQty, 6) * z.Qty;
                        var remainingQtyLost = Math.Round(qty / totalManualQty, 6) * z.QtyLost;
                        if (remainingQty != 0 || remainingQtyLost != 0)
                        {
                            batches.Add(new BatchReturnComponentProduction()
                            {
                                AdmissionDate = z.AdmissionDate,
                                BatchCode = z.BatchCode ?? "",
                                ExpDate = z.ExpDate,
                                Qty = remainingQty,
                                QtyLost = remainingQtyLost,
                            });

                            if (remainingQty <= z.Qty)
                            {
                                line.Qty = line.Qty - remainingQty;
                                remainingQty = 0;
                            }
                            else if (z.Qty <= remainingQty)
                            {
                                remainingQty -= z.Qty;
                                line.Qty = 0;
                            }

                            if (remainingQtyLost <= z.QtyLost)
                            {
                                line.QtyLost = line.QtyLost - remainingQtyLost;
                                remainingQtyLost = 0;
                            }
                            else if (z.QtyLost <= remainingQtyLost)
                            {
                                remainingQtyLost -= z.QtyLost;
                                line.QtyLost = 0;
                            }

                            if (z.Qty == 0 && z.QtyLost == 0)
                            {
                                line.Batches.Remove(z);
                            }
                        }

                        if (batches.Count > 0)
                        {
                            x.Batches = batches;
                        }
                    }
                }

                if (ViewModel.ReceiptFromProductionOrderForm.Lines
                        .Where(all =>
                            all.Batches != null && all.ItemCode == x.ItemCode
                                                && all.DocNum == x.DocNum)
                        .Sum(t1 => t1.Batches?.Sum(x => x.Qty)) > 0)
                {
                    return;
                }
                foreach (var z in line.Batches
                             .Where(all =>
                                 all.OnSelectedType.FirstOrDefault()?.Id == 1)
                             .ToList())
                {
                    var batches = new List<BatchReturnComponentProduction>();

                    var totalManualQty = ViewModel.GetProductionOrderLines!.Where(all =>
                            all.ItemCode == line.ItemCode
                            && all.DocEntry == x.DocNum)
                        .Sum(all => Convert.ToDouble(all.Qty));
                    totalManualQty += ViewModel.GetProductionOrderLines.Where(zz =>
                            (ViewModel.ReceiptFromProductionOrderForm.Lines
                                .Where(all =>
                                    all.ItemCode == line.ItemCode
                                    && all.Batches != null
                                    && all.Batches.Any(x => x.Qty == 0))
                                .Select(all => all.DocNum)).Contains(zz.DocEntry))
                        .Sum(zz1 => Convert.ToDouble(zz1.Qty ?? "0"));
                    var qty = Convert.ToDouble(ViewModel.GetProductionOrderLines!.FirstOrDefault(all =>
                        all.ItemCode == line.ItemCode
                        && all.DocEntry == x.DocNum
                        && all.OrderLineNum == x.BaseLineNum.ToString())?.Qty ?? "0");

                    var remainingQty = (qty / totalManualQty) * totalQtyAutoLost;

                    if (remainingQty != 0)
                    {
                        batches.Add(new BatchReturnComponentProduction()
                        {
                            AdmissionDate = z.AdmissionDate,
                            BatchCode = z.BatchCode ?? "",
                            ExpDate = z.ExpDate,
                            Qty = remainingQty,
                        });

                        if (remainingQty <= z.Qty)
                        {
                            z.Qty = z.Qty - remainingQty;
                            remainingQty = 0;
                        }
                        else if (z.Qty <= remainingQty)
                        {
                            remainingQty -= z.Qty;
                            line.Qty = 0;
                        }

                        if (z.Qty == 0)
                        {
                            line.Batches!.Remove(z);
                        }
                    }

                    if (batches.Count > 0)
                    {
                        if (x.Batches == null)
                            x.Batches = batches;
                        else
                            x.Batches?.AddRange(batches);
                    }
                    tmp.Add(x);
                    ViewModel.ReceiptFromProductionOrderForm.Lines.Remove(x);
                }
            });
            ViewModel.ReceiptFromProductionOrderForm.Lines.AddRange(tmp);
        }
    }
    void ProcessItemSerial(ReturnComponentProductionLine line)
    {
        if (line.Serials != null)
        {
            foreach (var lineManual in line.Serials)
            {
                ViewModel.ReceiptFromProductionOrderForm.Lines.Add(new ReturnComponentProductionLine
                {
                    DocNum = line.DocNum,
                    BaseLineNum = line.BaseLineNum,
                    ItemCode = line.ItemCode,
                    ItemName = line.ItemName,
                    Qty = lineManual.Qty,
                    QtyRequire = line.QtyRequire,
                    QtyPlan = line.QtyPlan,
                    QtyManual = lineManual.Qty,
                    Price = line.Price,
                    WhsCode = line.WhsCode,
                    Serials = line.Serials,
                });
            }
        }
    }
    #endregion
}
