using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Shared;
using Tri_Wall.Shared.ViewModels;
using Tri_Wall.Shared.Views;

namespace Tri_Wall.Shared.Pages.GoodReceptPo;

public partial class GoodReceptPoForm
{
    private string stringDisplay = "Recept";
    string? dataGrid = "width: 100%;height:405px";
    string? series;
    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();
    async Task OpenAsync()
    {
        var dictionary = new Dictionary<string, object>();
        dictionary.Add("item", ViewModel.Items);
        dictionary.Add("taxPurchase", ViewModel.TaxPurchases);
        dictionary.Add("warehouse", ViewModel.Warehouses);
        var dialog = await dialogService.ShowDialogAsync<DialogAddLine>(dictionary, new DialogParameters()
        {
            Title = $"Add Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        });
        var result = await dialog.Result;
        if (!result.Cancelled && result.Data != null)
        {
           var DialogData = (GoodReceiptPOLine)result.Data;
            Console.WriteLine(DialogData.WarehouseCode);
        }

    }
    private void OnSearch(OptionsSearchEventArgs<Vendors> e)
    {
        e.Items = ViewModel.Vendors.Where(i => i.VendorCode.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                            i.VendorName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(i => i.VendorCode);
    }
    void OnBreakpointEnterHandler(GridItemSize size)
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
