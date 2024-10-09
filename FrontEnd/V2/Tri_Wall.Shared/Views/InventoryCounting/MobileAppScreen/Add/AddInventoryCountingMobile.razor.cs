using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryCounting;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.Shared.Component;

namespace Tri_Wall.Shared.Views.InventoryCounting.MobileAppScreen.Add;

public partial class AddInventoryCountingMobile
{
    [Parameter] public int DocEntry { get; set; }
    [Inject] public IValidator<InventoryCountingHeader>? Validator { get; init; }
    Dictionary<string, object> _lineItemContent = new();
    bool _isItemLineClickAdd;
    private bool Visible { get; set; }

    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("inventorycounting");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        ComponentAttribute.Title = "List Search";
        ComponentAttribute.Path = "/inventorycounting";
        ComponentAttribute.IsBackButton = true;
        await ViewModel.LoadingCommand.ExecuteAsync(null).ConfigureAwait(false);
    }

    async Task<ObservableCollection<GetBatchOrSerial>> GetSerialBatch(Dictionary<string, string> dictionary)
    {
        await ViewModel.GetBatchOrSerialByItemCodeCommand.ExecuteAsync(dictionary);
        return ViewModel.GetBatchOrSerialsByItemCode;
    }

    private Task OnAddLineItem(InventoryCountingLine inventoryCounting)
    {
        _lineItemContent = new Dictionary<string, object>
        {
            { "item", ViewModel.GetInventoryCountingLines },
            { "warehouse", ViewModel.Warehouses },
            { "line", inventoryCounting },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };
        if (inventoryCounting.LineNum != 0)
        {
            _lineItemContent.Add("OnDeleteLineItem", new Func<int, Task>(OnDeleteItem));
        }

        _isItemLineClickAdd = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnDeleteItem(int lineNum)
    {
        ToastService.ShowToast<ToastCustom, Dictionary<string, object>>(
            new ToastParameters<Dictionary<string, object>>()
            {
                Intent = ToastIntent.Custom,
                Title = "Delete Item",
                Timeout = 6000,
                Icon = (new Icons.Regular.Size20.Delete(), Color.Accent),
                Content = new Dictionary<string, object>
                {
                    {
                        "Body", "Are you sure to Delete?"
                    },
                    {
                        "Index", lineNum
                    },
                    {
                        "OnClickPrimaryButton", new Func<Dictionary<string, object>, Task>(OnDeleteItemByLineNum)
                    },
                    {
                        "PrimaryButtonText", "Delete"
                    },
                    {
                        "ButtonPrimaryColor", "var(--bs-green)"
                    },
                    {
                        "ButtonSecondaryColor", "var(--bs-red)"
                    }
                }
            });

        return Task.CompletedTask;
    }

    Task OnDeleteItemByLineNum(Dictionary<string, object> dictionary)
    {
        ViewModel.InventoryCountingLines.RemoveAt((int)dictionary["Index"]);
        FluentToast fluentToast = (FluentToast)dictionary["FluentToast"];
        fluentToast.Close();
        OnAddItemLineBack();
        return Task.CompletedTask;
    }

    Task OnAddItemLineBack()
    {
        _isItemLineClickAdd = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    Task OnSaveItem(InventoryCountingLine inventoryCountingLine)
    {
        if (inventoryCountingLine.LineNum == 0)
        {
            inventoryCountingLine.LineNum =
                ViewModel.InventoryCountingHeader.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
            Console.WriteLine(JsonSerializer.Serialize(inventoryCountingLine));
            ViewModel.InventoryCountingHeader.Lines ??= new();
            ViewModel.InventoryCountingHeader.Lines?.Add(inventoryCountingLine);
            Console.WriteLine(JsonSerializer.Serialize(ViewModel.InventoryCountingHeader));
        }
        else
        {
            var index = ViewModel.InventoryCountingHeader.Lines!.FindIndex(i =>
                i.LineNum == inventoryCountingLine.LineNum);
            ViewModel.InventoryCountingHeader.Lines[index] = inventoryCountingLine;
        }

        OnAddItemLineBack();
        return Task.CompletedTask;
    }

    Task OnConfirmTransactionDialog()
    {
        ToastService.ShowToast<ToastCustom, Dictionary<string, object>>(
            new ToastParameters<Dictionary<string, object>>()
            {
                Intent = ToastIntent.Custom,
                Title = "Confirmation",
                Timeout = 6000,
                Icon = (new Icons.Regular.Size20.CubeAdd(), Color.Accent),
                Content = new Dictionary<string, object>
                {
                    {
                        "Body", "Are you sure to Confirm?"
                    },
                    {
                        "Index", 1
                    },
                    {
                        "OnClickPrimaryButton", new Func<Dictionary<string, object>, Task>(OnConfirmTransaction)
                    },
                    {
                        "PrimaryButtonText", "Confirm"
                    },
                    {
                        "ButtonPrimaryColor", "var(--bs-green)"
                    },
                    {
                        "ButtonSecondaryColor", "var(--bs-red)"
                    }
                }
            });
        return Task.CompletedTask;
    }

    async Task OnConfirmTransaction(Dictionary<string, object> dictionary)
    {
        FluentToast fluentToast = (FluentToast)dictionary["FluentToast"];
        fluentToast.Close();
        await ErrorHandlingHelper.ExecuteWithHandlingAsync(async () =>
        {
            ViewModel.InventoryCountingHeader.CreateDate = DateTime.Now;
            var result = await Validator!.ValidateAsync(ViewModel.InventoryCountingHeader).ConfigureAwait(false);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ToastService!.ShowError(error.ErrorMessage);
                }

                return;
            }

            Visible = true;
            StateHasChanged();
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (ViewModel.PostResponses.ErrorCode == "")
            {
                SelectedProductionOrder = new List<GetInventoryCountingList>();
                ViewModel.InventoryCountingHeader = new();
                ViewModel.InventoryCountingLines = new();
                ToastService.ShowSuccess("Success");
                // if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
        }, ViewModel.PostResponses, ToastService).ConfigureAwait(false);
        Visible = false;
        StateHasChanged();
    }

    private void OnSearch(OptionsSearchEventArgs<GetInventoryCountingList> e)
    {
        e.Items = ViewModel.GetInventoryCountingLists.Where(i => i.Series.Contains(e.Text,
                                                                     StringComparison.OrdinalIgnoreCase) ||
                                                                 i.Series.Contains(e.Text
                                                                     , StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.Series);
    }

    private IEnumerable<GetInventoryCountingList> SelectedProductionOrder { get; set; } =
        new List<GetInventoryCountingList>();

    private async Task UpdateItemDetails(string? newValue)
    {
        if (!SelectedProductionOrder.Any())
        {
            ViewModel.InventoryCountingHeader.DocEntry = 0;
            ViewModel.InventoryCountingHeader.Series = "";
            ViewModel.InventoryCountingHeader.CreateDate = DateTime.Now;
            ViewModel.InventoryCountingHeader.CreateTime = "";
            ViewModel.InventoryCountingHeader.OtherRemark = "";
            ViewModel.InventoryCountingHeader.Ref2 = "";
            ViewModel.InventoryCountingHeader.InventoryCountingType = "";
            return;
        }

        ViewModel.InventoryCountingHeader.DocEntry = Convert.ToInt32(SelectedProductionOrder.ToList()[0].DocEntry);
        ViewModel.InventoryCountingHeader.Series = SelectedProductionOrder.ToList()[0].Series;
        ViewModel.InventoryCountingHeader.CreateDate =
            Convert.ToDateTime(SelectedProductionOrder.ToList()[0].CreateDate);
        ViewModel.InventoryCountingHeader.CreateTime = SelectedProductionOrder.ToList()[0].CreateTime;
        ViewModel.InventoryCountingHeader.OtherRemark = SelectedProductionOrder.ToList()[0].OtherRemark;
        ViewModel.InventoryCountingHeader.Ref2 = SelectedProductionOrder.ToList()[0].Ref2;
        ViewModel.InventoryCountingHeader.InventoryCountingType =
            SelectedProductionOrder.ToList()[0].InventoryCountingType;
        await ViewModel.GetPurchaseOrderLineByDocEntryCommand.ExecuteAsync(SelectedProductionOrder.ToList()[0].DocEntry)
            .ConfigureAwait(false);
        StateHasChanged();
    }

    protected void OnCloseOverlay() => Visible = true;
}