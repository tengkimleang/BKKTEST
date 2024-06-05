
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
namespace Tri_Wall.Shared.Views.DeliveryOrder;

public partial class DialogAddLineGoodDeliveryOrder
{
    [Inject]
    public IValidator<DeliveryOrderLine>? Validator { get; init; }

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public Dictionary<string, object> Content { get; set; } = default!;

    private DeliveryOrderLine DataResult { get; set; } = new();
    private List<BatchDeliveryOrder> batchReceiptPOs = new();
    private List<SerialDeliveryOrder> serialReceiptPO = new();
    private IEnumerable<GetBatchOrSerial> _serialBatchDeliveryOrders=new List<GetBatchOrSerial>();
    private bool _isItemBatch;
    private bool _isItemSerial;
    private IEnumerable<Items> _selectedItem = Array.Empty<Items>();
    private IEnumerable<GetBatchOrSerial> _selectedSerialDeliveryOrders = Array.Empty<GetBatchOrSerial>();
    private IEnumerable<Items> _items => Content["item"] as IEnumerable<Items> ?? new List<Items>();
    
    private Func<Dictionary<string,string>, Task<ObservableCollection<GetBatchOrSerial>>> GetSerialBatch => Content["getSerialBatch"] as Func<Dictionary<string,string>, Task<ObservableCollection<GetBatchOrSerial>>>?? default!;
    private IEnumerable<VatGroups>? _vatGroups => Content["taxPurchase"] as IEnumerable<VatGroups>;
    private IEnumerable<Warehouses>? _warehouses => Content["warehouse"] as IEnumerable<Warehouses>;
    string? dataGrid = "width: 1600px;overflow-x:scroll;";

    override protected void OnInitialized()
    {
        if (Content.ContainsKey("line"))
        {
            DataResult = Content["line"] as DeliveryOrderLine ?? new DeliveryOrderLine();
            batchReceiptPOs = DataResult.Batches ?? new List<BatchDeliveryOrder>();
            serialReceiptPO = DataResult.Serials ?? new List<SerialDeliveryOrder>();
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
    
    private void OnSearchSerial(OptionsSearchEventArgs<GetBatchOrSerial> e)
    {
        e.Items = _serialBatchDeliveryOrders?.Where(i => i.SerialBatch.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                                                    i.SerialBatch.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.SerialBatch);
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

    private async Task UpdateItemDetails(string newValue)
    {
        var firstItem = _selectedItem.FirstOrDefault();
        DataResult.Price = (DataResult.Price==0)?double.Parse(firstItem?.PriceUnit ?? "0"): DataResult.Price;
        DataResult.ItemCode = firstItem?.ItemCode ?? "";
        DataResult.ItemName = firstItem?.ItemName ?? "";
        DataResult.ManageItem = firstItem?.ItemType;
        _isItemBatch = firstItem?.ItemType == "B";
        _isItemSerial = firstItem?.ItemType == "S";
        if (firstItem?.ItemType != "N")
            _serialBatchDeliveryOrders=await GetSerialBatch(new Dictionary<string, string>
                {
                    {"ItemCode", firstItem?.ItemCode ??""},
                    {"ItemType", firstItem?.ItemType ??""}
                });
       
    }
    private void OnSelectedSerialOrBatch(string newValue,int index)
    {
        var firstItem = _serialBatchDeliveryOrders.FirstOrDefault();
        if(firstItem?.Type == "Batch")
        {
            batchReceiptPOs[index].BatchCode = firstItem.SerialBatch;
            batchReceiptPOs[index].Qty = 0;
            batchReceiptPOs[index].QtyAvailable = Convert.ToInt32(firstItem.Qty);
            batchReceiptPOs[index].ManfectureDate = Convert.ToDateTime(firstItem.MrfDate??null);
            batchReceiptPOs[index].ExpDate = Convert.ToDateTime(firstItem.ExpDate ?? null);
        }else if(firstItem?.Type == "Serial")
        {
            serialReceiptPO[index].SerialCode = firstItem.SerialBatch;
            serialReceiptPO[index].Qty = Convert.ToInt32(firstItem.Qty);
            serialReceiptPO[index].MfrDate = Convert.ToDateTime(firstItem.MrfDate ?? null);
            serialReceiptPO[index].ExpDate = Convert.ToDateTime(firstItem.ExpDate ?? null);
        }
    }

    private void AddLineToBatchOrSerial()
    {
        if (_isItemBatch)
        {
            batchReceiptPOs.Add(new BatchDeliveryOrder { BatchCode = "", });
        }
        else if (_isItemSerial && serialReceiptPO.Count() < DataResult.Qty)
        {
            serialReceiptPO.Add(new SerialDeliveryOrder { SerialCode = "", });
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
        dataGrid = size == GridItemSize.Xs ? "width: 1600px;height:205px" : "width: 2600px;height:405px";
    }
}
