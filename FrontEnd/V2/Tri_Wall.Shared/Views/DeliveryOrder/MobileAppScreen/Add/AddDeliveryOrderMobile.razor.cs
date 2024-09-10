using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.Shared.Component;

namespace Tri_Wall.Shared.Views.DeliveryOrder.MobileAppScreen.Add;

public partial class AddDeliveryOrderMobile
{
    [Inject] public IValidator<DeliveryOrderHeader>? Validator { get; init; }
    IEnumerable<Vendors> _selectedVendor = Array.Empty<Vendors>();
    Dictionary<string, object> _lineItemContent = new();
    bool _isItemLineClickAdd;
    private bool Visible { get; set; }

    protected override void OnInitialized()
    {
        ComponentAttribute.Title = "List Search";
        ComponentAttribute.Path = "/deliveryorder";
        ComponentAttribute.IsBackButton = true;
    }

    private void OnSearch(OptionsSearchEventArgs<Vendors> e)
    {
        e.Items = ViewModel.Customers.Where(i => i.VendorCode.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                                                 i.VendorName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.VendorCode);
    }

    async Task<ObservableCollection<GetBatchOrSerial>> GetSerialBatch(Dictionary<string, string> dictionary)
    {
        await ViewModel.GetBatchOrSerialByItemCodeCommand.ExecuteAsync(dictionary);
        return ViewModel.GetBatchOrSerialsByItemCode;
    }

    private Task OnAddLineItem(DeliveryOrderLine deliveryOrderLine)
    {
        _lineItemContent = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxSales },
            { "warehouse", ViewModel.Warehouses },
            { "line", deliveryOrderLine },
            {
                "getSerialBatch",
                new Func<Dictionary<string, string>, Task<ObservableCollection<GetBatchOrSerial>>>(GetSerialBatch)
            }
        };
        if (deliveryOrderLine.LineNum != 0)
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
                        "PrimaryButtonText" , "Delete"
                    }
                }
            });

        return Task.CompletedTask;
    }

    Task OnDeleteItemByLineNum(Dictionary<string, object> dictionary)
    {
        ViewModel.DeliveryOrderForm.Lines?.RemoveAll(x => x.LineNum == (int)dictionary["Index"]);
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

    Task OnSaveItem(DeliveryOrderLine deliveryOrderLine)
    {
        if (deliveryOrderLine.LineNum == 0)
        {
            deliveryOrderLine.LineNum = ViewModel.DeliveryOrderForm.Lines?.MaxBy(x => x.LineNum)?.LineNum + 1 ?? 1;
            Console.WriteLine(JsonSerializer.Serialize(deliveryOrderLine));
            ViewModel.DeliveryOrderForm.Lines ??= new();
            ViewModel.DeliveryOrderForm.Lines?.Add(deliveryOrderLine);
            Console.WriteLine(JsonSerializer.Serialize(ViewModel.DeliveryOrderForm));
        }
        else
        {
            var index = ViewModel.DeliveryOrderForm.Lines!.FindIndex(i => i.LineNum == deliveryOrderLine.LineNum);
            ViewModel.DeliveryOrderForm.Lines[index] = deliveryOrderLine;
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
                        "PrimaryButtonText" , "Confirm"
                    },
                    {
                        "ButtonPrimaryColor","var(--bs-green)"
                    },
                    {
                        "ButtonSecondaryColor","var(--bs-red)"
                    }
                }
            });
        return Task.CompletedTask;
    }

    async Task OnConfirmTransaction(Dictionary<string,object> dictionary)
    {
        FluentToast fluentToast = (FluentToast)dictionary["FluentToast"];
        fluentToast.Close();
        await ErrorHandlingHelper.ExecuteWithHandlingAsync(async () =>
        {   
            ViewModel.DeliveryOrderForm.CustomerCode = _selectedVendor.FirstOrDefault()?.VendorCode ?? "";
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

            Visible = true;
            StateHasChanged();
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (ViewModel.PostResponses.ErrorCode == "")
            {
                _selectedVendor = new List<Vendors>();
                ViewModel.DeliveryOrderForm = new DeliveryOrderHeader();
                ToastService.ShowSuccess("Success");
                // if (type == "print") await OnSeleted(ViewModel.PostResponses.DocEntry.ToString());
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
        }, ViewModel.PostResponses, ToastService).ConfigureAwait(false);
        Visible = false;
        StateHasChanged();
    }
    protected void OnCloseOverlay() => Visible = true;
}