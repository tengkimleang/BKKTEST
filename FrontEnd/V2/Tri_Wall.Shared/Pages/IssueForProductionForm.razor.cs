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

public partial class IssueForProductionForm
{
    [Inject] public IValidator<IssueProductionHeader>? Validator { get; init; }
    [Inject] public IValidator<IssueProductionLine>? ValidatorLine { get; init; }

    private string stringDisplay = "Issue For Production";
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
            stringDisplay = "Issue For Production";
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
        var strMP = JsonSerializer.Serialize(ViewModel.IssueProductionLine.AsQueryable());
        var strMP1 = JsonSerializer.Serialize(ViewModel.GetProductionOrderLines.AsQueryable());
        ViewModel.IssueProduction.Lines = new();
        foreach (var line in issueProductionLines)
        {
            var total = ViewModel.GetProductionOrderLines?.Where(x => x.ItemCode ==
                                                                      line.ItemCode)
                .Sum(x => Convert.ToDouble(x.Qty)) ?? 0;
            foreach (var vmIssueProductionLine in ViewModel.GetProductionOrderLines!.Where(x =>
                         x.ItemCode == line.ItemCode).ToList())
            {
                var actualQty = (Convert.ToDouble(vmIssueProductionLine.Qty ?? "0") / total) * line.Qty;
                if (line.ManageItem == "S")
                {
                    var serials = new List<SerialIssueProduction>();
                    for (var i = 0; i < actualQty; i++)
                    {
                        if (line.Serials?.Count > 0)
                        {
                            serials.Add(line.Serials.FirstOrDefault()!);
                            line.Serials.Remove(line.Serials.FirstOrDefault()!);
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
                else if (line.ManageItem == "B")
                {
                    var batches = new List<BatchIssueProduction>();
                    //var qty = actualQty;
                    var remainingQty = actualQty;
                    foreach (var batch in line.Batches!.ToList())
                    {
                        Console.WriteLine(batch.Qty);
                        if (remainingQty != 0)
                        {
                            var index = line.Batches!.IndexOf(batch);
                            var indexHeader= issueProductionLines.IndexOf(line);
                            batches.Add(new BatchIssueProduction
                            {
                                AdmissionDate = batch.AdmissionDate,
                                BatchCode = batch.BatchCode ?? "",
                                ExpDate = batch.ExpDate,
                                Qty = remainingQty
                            });

                            if (batch.Qty> remainingQty)
                            {
                                //line.Batches![index].Qty -= remainingQty;
                                issueProductionLines[indexHeader].Batches![index].Qty -= remainingQty;
                                remainingQty = 0;
                            }   
                            else if(batch.Qty <= remainingQty)
                            {
                                remainingQty -= batch.Qty;
                                //line.Batches![index].Qty = 0;
                                issueProductionLines[indexHeader].Batches![index].Qty = 0;
                            }
                            Console.WriteLine(issueProductionLines[indexHeader]?.Batches?[index].Qty);
                            //Console.WriteLine(line.Batches![index].Qty);
                            if (batch.Qty == 0)
                            {
                                line.Batches!.Remove(batch);
                            }
                            //if (batch.Qty >= actualQty)
                            //{
                            //    batch.Qty -= actualQty;
                            //    actualQty = 0;
                            //    if (batch.Qty == 0)
                            //    {
                            //        var index = line.Batches!.ToList().FindIndex(x => x.BatchCode == batch.BatchCode);
                            //        issueProductionLines[index].Batches!.ToList().Remove(batch);
                            //    }
                            //}
                            //else
                            //{
                            //    actualQty -= batch.Qty;
                            //    batch.Qty = 0;
                            //    var index = line.Batches!.ToList().FindIndex(x => x.BatchCode == batch.BatchCode);
                            //    issueProductionLines[index].Batches!.ToList().Remove(batch);
                            //}

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
                else
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
            }
        }

        ViewModel.IssueProductionLine = new();

        ViewModel.IssueProductionLine = JsonSerializer.Deserialize<ObservableCollection<IssueProductionLine>>(strMP) ?? new();
        ViewModel.GetProductionOrderLines = JsonSerializer.Deserialize<ObservableCollection<GetProductionOrderLines>>(strMP1) ?? new();

        Console.WriteLine(JsonSerializer.Serialize(ViewModel.IssueProduction));
        //var result = await Validator!.ValidateAsync(ViewModel.IssueProduction).ConfigureAwait(false);

        //if (!result.IsValid)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ToastService!.ShowError(error.ErrorMessage);
        //    }

        //    return;
        //}

        //try
        //{
        //    visible = true;

        //    await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

        //    if (ViewModel.PostResponses.ErrorCode == "")
        //    {
        //        SelectedProductionOrder = new List<GetProductionOrder>();
        //        ViewModel.IssueProduction = new();
        //        ViewModel.IssueProductionLine = new();
        //        ToastService.ShowSuccess("Success");
        //        if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
        //    }
        //    else
        //        ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);

        //    visible = false;
        //}
        //catch (ApiException ex)
        //{
        //    var content = ex.GetContentAsAsync<Dictionary<String, String>>();
        //    ToastService!.ShowError(ex.ReasonPhrase ?? "");
        //    visible = false;
        //}
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