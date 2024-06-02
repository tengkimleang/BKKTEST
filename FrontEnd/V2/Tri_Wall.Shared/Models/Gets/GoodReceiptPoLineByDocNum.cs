

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
    string ManageItem
    );
