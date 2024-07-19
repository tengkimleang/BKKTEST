

namespace Tri_Wall.Shared.Models.Gets;

public record GoodReceiptPoLineByDocNum(
    string BaseLineNumber,
    string DocEntry,
    string ItemCode,
    string ItemName,
    string Qty,
    string Price,
    string LineTotal,
    string VatCode,
    string WarehouseCode,
    string BarCode,
    string ManageItem,
    List<BatchGoodReceiptPoCopyFrom>? Batches,
    List<SerialGoodReceiptPoCopyFrom>? Serials
    );

public class BatchGoodReceiptPoCopyFrom
{
    public string BatchCode { get; set; } = string.Empty;
    public double Qty { get; set; }
    public double QtyAvailable { get; set; }
    public DateTime? ExpDate { get; set; } = DateTime.Today;
    public DateTime? ManfectureDate { get; set; } = DateTime.Today;
    public DateTime? AdmissionDate { get; set; } = DateTime.Today;
    public string LotNo { get; set; } = string.Empty;
    public IEnumerable<GetBatchOrSerial> OnSelectedBatchOrSerial { get; set; } = Array.Empty<GetBatchOrSerial>();
}

public class SerialGoodReceiptPoCopyFrom
{
    public string SerialCode { get; set; } = string.Empty;
    public int Qty { get; set; } = 1;
    public string MfrNo { get; set; } = string.Empty;
    public DateTime? MfrDate { get; set; } = DateTime.Today;
    public DateTime? ExpDate { get; set; } = DateTime.Today;
    public IEnumerable<GetBatchOrSerial> OnSelectedBatchOrSerial { get; set; } = Array.Empty<GetBatchOrSerial>();
}
