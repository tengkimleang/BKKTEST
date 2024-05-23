using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Views;

namespace Tri_Wall.Shared.Pages.GoodReceptPo;

public partial class GoodReceptPoForm
{
    [Inject]
    public IDialogService? DialogService { get; init; }
    private string stringDisplay = "Recept";
    string? dataGrid = "width: 100%;height:405px";
    string? series;
    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    async Task OpenDialogAsync()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLine>(dictionary, new DialogParameters
        {
            Title = "Add Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        });

        var result = await dialog.Result;
        if (!result.Cancelled && result.Data is GoodReceiptPoLine line)
        {
            ViewModel.GoodReceiptPOForm.Lines?.Add(line);
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
}
