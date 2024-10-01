﻿

using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryTransfer;

namespace Tri_Wall.Shared.Views.InventoryTransfer.MobileAppScreen.Add;

public partial class AddSerialLine
{
    [Parameter] public Func<Task> IsViewDetail { get; set; } = default!;
    [Parameter] public Func<SerialInventoryTransfer, Task> SaveSerial { get; set; } = default!;
    [Parameter] public Func<int,Task> DeleteSerial { get; set; } = default!;

    [Parameter]
    public IEnumerable<GetBatchOrSerial> SerialBatchDeliveryOrders { get; set; } = new List<GetBatchOrSerial>();

    [Parameter] public int Index { get; set; }

    [Parameter] public bool IsUpdate { get; set; }

    [Parameter] public IEnumerable<GetBatchOrSerial> SelectedSerial { get; set; } = Array.Empty<GetBatchOrSerial>();
    private SerialInventoryTransfer SerialInventoryTransfer { get; set; } = new();

    protected override void OnInitialized()
    {
        Console.WriteLine(SelectedSerial.Count());
        if (SelectedSerial.Count() != 0)
            UpdateItemDetails("");
    }

    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("deliveryorder");
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
        var firstItem = SelectedSerial.FirstOrDefault();
        if (firstItem == null) return Task.CompletedTask;
        Console.WriteLine(JsonSerializer.Serialize(firstItem));
        SerialInventoryTransfer.SerialCode = firstItem.SerialBatch;
        SerialInventoryTransfer.Qty = 1;
        SerialInventoryTransfer.ExpDate = (!string.IsNullOrEmpty(firstItem.ExpDate))
            ? DateTime.Parse(firstItem.ExpDate)
            : SerialInventoryTransfer.ExpDate;
        SerialInventoryTransfer.MfrDate = (!string.IsNullOrEmpty(firstItem.MrfDate))
            ? DateTime.Parse(firstItem.MrfDate)
            : SerialInventoryTransfer.MfrDate;
        return Task.CompletedTask;
    }
}