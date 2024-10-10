using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryCounting;

namespace Tri_Wall.Shared.Views.InventoryCounting.MobileAppScreen.Add;

public partial class AddBatchLine
{
    [Parameter] public Func<Task> IsViewDetail { get; set; } = default!;
    [Parameter] public Func<InventoryCountingBatch, Task> SaveBatch { get; set; } = default!;
    [Parameter] public Func<int,Task> DeleteBatch { get; set; } = default!;

    [Parameter]
    public IEnumerable<GetBatchOrSerial> SerialBatchDeliveryOrders { get; set; } = new List<GetBatchOrSerial>();

    [Parameter] public int Index { get; set; }

    [Parameter] public bool IsUpdate { get; set; }

    [Parameter] public IEnumerable<GetBatchOrSerial> SelectedBatch { get; set; } = Array.Empty<GetBatchOrSerial>();
    private InventoryCountingBatch InventoryCountingBatch { get; set; } = new();
    
    private IEnumerable<ItemType> _type = new List<ItemType>
    {
        new ItemType
        {
            Id = 1,
            Name = "New"
        },
        new ItemType
        {
            Id = 2,
            Name = "Existing"
        }
    };

    private IEnumerable<ItemType> SelectedType { get; set; } = new List<ItemType> { new ItemType { Id = 2, Name = "Existing" } };
    public bool DisplayNoneOrShow;

    protected override void OnInitialized()
    {
        Console.WriteLine(SelectedBatch.Count());
        if (SelectedBatch.Count() != 0)
            UpdateItemDetails("");
    }

    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("inventorycounting");
        }
    }

    private void OnSearchBatch(OptionsSearchEventArgs<GetBatchOrSerial> e)
    {
        e.Items = SerialBatchDeliveryOrders
            .Where(i => i.SerialBatch.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.SerialBatch);
    }

    private Task UpdateItemDetails(string newValue)
    {
        var firstItem = SelectedBatch.FirstOrDefault();
        if (firstItem == null) return Task.CompletedTask;
        Console.WriteLine(JsonSerializer.Serialize(firstItem));
        InventoryCountingBatch.BatchCode = firstItem.SerialBatch;
        InventoryCountingBatch.Qty = (string.IsNullOrEmpty(firstItem.InputQty)) ? 0 : Convert.ToDouble(firstItem.InputQty);
        InventoryCountingBatch.ExpireDate = (!string.IsNullOrEmpty(firstItem.ExpDate))
            ? DateTime.Parse(firstItem.ExpDate)
            : InventoryCountingBatch.ExpireDate;
        InventoryCountingBatch.ManufactureDate = (!string.IsNullOrEmpty(firstItem.MrfDate))
            ? DateTime.Parse(firstItem.MrfDate)
            : InventoryCountingBatch.ManufactureDate;
        InventoryCountingBatch.AdmissionDate = (!string.IsNullOrEmpty(firstItem.MrfDate))
            ? DateTime.Parse(firstItem.MrfDate)
            : InventoryCountingBatch.AdmissionDate;
        InventoryCountingBatch.QtyAvailable = Convert.ToDouble(firstItem.Qty);
        return Task.CompletedTask;
    }
    private void OnSearchType(OptionsSearchEventArgs<ItemType> e)
    {
        e.Items = _type.Where(i => i.Name.Contains(e.Text, StringComparison.OrdinalIgnoreCase));
    }
    private void OnSelectedType(string newValue)
    {
        DisplayNoneOrShow = SelectedType.FirstOrDefault()?.Id == 1;
        StateHasChanged();
    }
}