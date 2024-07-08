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
using Tri_Wall.Shared.Models.ReturnComponentProduction;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.IssueForProduction;
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
            if (ViewModel.IssueProduction?.Lines == null)
                ViewModel.IssueProduction!.Lines = new List<ReturnComponentProductionLine>();
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
        ViewModel.IssueProduction.Lines!.RemoveAt(index);
    }

    async Task OnSaveTransaction(string type = "")
    {
        var productionOrder = ViewModel.IssueProductionLine.ToList();
        ViewModel.IssueProduction.Lines = new();
        var strMP = JsonSerializer.Serialize(ViewModel.IssueProductionLine.AsQueryable());
        //Console.WriteLine(JsonSerializer.Serialize(ViewModel.GetProductionOrderLines));
        //Console.WriteLine(JsonSerializer.Serialize(productionOrder));
        foreach (var line in productionOrder.ToList())
        {
            if (line.ManageItem == "N")
            {
                if (line.ItemNones != null)
                {
                    foreach (var lineManual in line.ItemNones)
                    {
                        //Console.WriteLine("lineManual");
                        //Console.WriteLine(JsonSerializer.Serialize(lineManual));
                        //Console.WriteLine(JsonSerializer.Serialize(ViewModel.GetProductionOrderLines!.Where(x=> 
                        //    x.ItemCode==line.ItemCode 
                        //    && x.DocEntry==lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry
                        //    )));
                        var totalManualQty = ViewModel.GetProductionOrderLines!.Where(x =>
                                    x.ItemCode == line.ItemCode
                                    && x.DocEntry == lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry
                                    )
                                    .Sum(x => Convert.ToDouble(x.Qty));
                        foreach (var AddLineManual in ViewModel.GetProductionOrderLines!.Where(x =>
                            x.ItemCode == line.ItemCode
                            && x.DocEntry == lineManual.OnSelectedProductionOrder.FirstOrDefault()?.DocEntry))
                        {
                            //Console.WriteLine((Convert.ToDouble(AddLineManual.Qty ?? "0") / totalManualQty) * lineManual.Qty);
                            //Console.WriteLine((Convert.ToDouble(AddLineManual.Qty ?? "0") / total) * lineManual.QtyLost);
                            ViewModel.IssueProduction.Lines.Add(new ReturnComponentProductionLine
                            {
                                DocNum = AddLineManual.DocEntry,
                                //LineNum = line.LineNum,
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
                                UomName = "Testing123",
                            });
                        }
                    }
                    productionOrder.Remove(line);
                }
                else
                {
                    var total = ViewModel.GetProductionOrderLines!.Where(x =>
                                    x.ItemCode == line.ItemCode
                                    && !ViewModel.IssueProduction.Lines.Where(z => z.Qty > 0).Select(x => x.DocNum).Contains(x.DocEntry)
                                    )
                                    .Sum(x => Convert.ToDouble(x.Qty));
                    Console.WriteLine(JsonSerializer.Serialize(ViewModel.GetProductionOrderLines!.Where(x =>
                                x.ItemCode == line.ItemCode
                                && !ViewModel.IssueProduction.Lines.Where(z => z.Qty > 0).Select(x => x.DocNum).Contains(x.DocEntry)
                                )));
                    //foreach (var AddLineManual in ViewModel.GetProductionOrderLines!.Where(x =>
                    //            x.ItemCode == line.ItemCode
                    //            && ViewModel.IssueProduction.Lines.Where(z => z.Qty > 0
                    //                                                          && z.ItemCode != x.ItemCode
                    //                                                          && z.DocNum == x.DocEntry
                    //                                                          && z.BaseLineNum == Convert.ToInt32(x.OrderLineNum ?? "0")
                    //            ).Count() == 0))
                    //{
                    //    //Console.WriteLine((Convert.ToDouble(AddLineManual.Qty ?? "0") / total) * line.Qty);
                    //    ViewModel.IssueProduction.Lines.Add(new ReturnComponentProductionLine
                    //    {
                    //        DocNum = AddLineManual.DocEntry,
                    //        //LineNum = line.LineNum,
                    //        BaseLineNum = Convert.ToInt32(AddLineManual.OrderLineNum ?? "0"),
                    //        ItemCode = line.ItemCode,
                    //        ItemName = line.ItemName,
                    //        Qty = (Convert.ToDouble(AddLineManual.Qty ?? "0") / total) * line.Qty,
                    //        QtyLost = (Convert.ToDouble(AddLineManual.Qty ?? "0") / total) * line.QtyLost,
                    //        Price = line.Price,
                    //        WhsCode = line.WhsCode,
                    //        UomName = "Testing",
                    //    });
                    //}
                    //productionOrder.Remove(line);
                }
            }
            
        }
        //Console.WriteLine(JsonSerializer.Serialize(ViewModel.IssueProduction.Lines));
        ViewModel.IssueProductionLine = JsonSerializer.Deserialize<ObservableCollection<ReturnComponentProductionLine>>(strMP) ?? new();
    }

    Task OnSeleted(string e)
    {
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
}
