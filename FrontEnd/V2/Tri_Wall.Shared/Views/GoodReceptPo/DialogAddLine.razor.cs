﻿
using Tri_Wall.Shared.Models.GoodReceiptPo;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
namespace Tri_Wall.Shared.Views.GoodReceptPo;

public partial class DialogAddLine
{
    [Inject]
    public IValidator<GoodReceiptPoLine>? Validator { get; init; }

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public Dictionary<string, object> Content { get; set; } = default!;

    private GoodReceiptPoLine DataResult { get; set; } = new();
    private List<BatchReceiptPo> batchReceiptPOs = new();
    private List<SerialReceiptPo> serialReceiptPO = new();
    private bool _isItemBatch;
    private bool _isItemSerial;
    private IEnumerable<Items> _selectedItem = Array.Empty<Items>();
    private IEnumerable<Items> _items => Content["item"] as IEnumerable<Items> ?? new List<Items>();
    private IEnumerable<VatGroups>? _vatGroups => Content["taxPurchase"] as IEnumerable<VatGroups>;
    private IEnumerable<Warehouses>? _warehouses => Content["warehouse"] as IEnumerable<Warehouses>;
    string? dataGrid = "width: 100%;";

    override protected void OnInitialized()
    {
        if (Content.ContainsKey("line"))
        {
            DataResult = Content["line"] as GoodReceiptPoLine ?? new GoodReceiptPoLine();
            batchReceiptPOs = DataResult.Batches ?? new List<BatchReceiptPo>();
            serialReceiptPO = DataResult.Serials ?? new List<SerialReceiptPo>();
            _selectedItem = _items.Where(i => i.ItemCode == DataResult.ItemCode);
            UpdateItemDetails(DataResult.ItemCode);
        }
    }

    private void OnSearch(OptionsSearchEventArgs<Items> e)
    {
        e.Items = _items?.Where(i => i.ItemCode.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                            i.ItemName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(i => i.ItemCode);
    }

    private async Task SaveAsync()
    {
        DataResult.Batches = batchReceiptPOs;
        DataResult.Serials = serialReceiptPO;
        var result = await Validator!.ValidateAsync(DataResult).ConfigureAwait(false);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }
            return;
        }
        await Dialog.CloseAsync(new Dictionary<string, object>
        {
            { "data", DataResult },
            { "isUpdate", Content.ContainsKey("line") }
        });
    }

    private void UpdateItemDetails(string newValue)
    {
        var firstItem = _selectedItem.FirstOrDefault();
        DataResult.Price = double.Parse(firstItem?.PriceUnit ?? "0");
        DataResult.ItemCode = firstItem?.ItemCode ?? "";
        DataResult.ItemName = firstItem?.ItemName ?? "";
        DataResult.ManageItem = firstItem?.ItemType;
        _isItemBatch = firstItem?.ItemType == "B";
        _isItemSerial = firstItem?.ItemType == "S";
    }

    private void AddLineToBatchOrSerial()
    {
        if (_isItemBatch)
        {
            batchReceiptPOs.Add(new BatchReceiptPo { BatchCode = "", });
        }
        else if (_isItemSerial && serialReceiptPO.Count() < DataResult.Qty)
        {
            serialReceiptPO.Add(new SerialReceiptPo { SerialCode = "", });
        }
    }

    private void DeleteLineFromBatchOrSerial(int index)
    {
        if (_isItemBatch)
        {
            batchReceiptPOs.RemoveAt(index);
        }
        else if (_isItemSerial)
        {
            serialReceiptPO.RemoveAt(index);
        }
    }

    private void UpdateGridSize(GridItemSize size)
    {
        dataGrid = size == GridItemSize.Xs ? "width: 1200px;height:205px" : "width: 100%;height:405px";
    }
}
