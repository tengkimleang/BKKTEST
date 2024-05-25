using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Views;

namespace Tri_Wall.Shared.Pages.GoodReceptPo;

public partial class GoodReceptPoForm
{
    [Inject]
    public IValidator<GoodReceiptPoHeader>? Validator { get; init; }
    private string stringDisplay = "Recept";
    string? dataGrid = "width: 100%;height:405px";
    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    async Task OpenDialogAsync(GoodReceiptPoLine goodReceiptPoLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses },
            { "line", goodReceiptPoLine }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLine>(dictionary, new DialogParameters
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
            if(ViewModel.GoodReceiptPOForm.Lines==null) ViewModel.GoodReceiptPOForm.Lines = new List<GoodReceiptPoLine>();
            if(data["data"] is GoodReceiptPoLine _goodReceiptPoLine)
            {
                if (_goodReceiptPoLine.LineNum == 0)
                {
                    _goodReceiptPoLine.LineNum = ViewModel.GoodReceiptPOForm.Lines.Count + 1;
                    ViewModel.GoodReceiptPOForm.Lines.Add(_goodReceiptPoLine);
                }
                else
                {
                    var index = ViewModel.GoodReceiptPOForm.Lines.FindIndex(i => i.LineNum == _goodReceiptPoLine.LineNum);
                    ViewModel.GoodReceiptPOForm.Lines[index] = _goodReceiptPoLine;
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
        ViewModel.GoodReceiptPOForm.Lines!.RemoveAt(index);
    }
    async Task OnSaveTransaction()
    {
        ViewModel.GoodReceiptPOForm.VendorCode = selectedVendor.FirstOrDefault()?.VendorCode ?? "";
        var result = await Validator!.ValidateAsync(ViewModel.GoodReceiptPOForm).ConfigureAwait(false);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }
            return;
        }
        await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);
    }
}
