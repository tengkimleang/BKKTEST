

namespace Tri_Wall.Shared.Models.Gets;

public record GetDetailInventoryCountingLineByDocNum(
        string LineNum,
        string ItemCode,
        string ItemName,
        string WarehouseCode,
        string Qty,
        string QtyCounted,
        string ManageItem
    );