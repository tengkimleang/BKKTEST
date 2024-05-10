

using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.InventoryCounting;

public record AddInventoryCountingCommand(
    int DocEntry,
    string Ref2,
    string Remarks,
    int Series,
    string OthRemark,
    List<Counter> Counters,
    List<InventoryCountingLine> Lines) : IRequest<ErrorOr<PostResponse>>;

public record Counter(int CountId);

public record InventoryCountingLine(
    string ItemCode,
    double Qty,
    string Counted,
    int LineNum,
    string ManageItem,
    int CountId,
    int BinEntry,
    string UoM,
    List<InventoryCountingBatch> Batches,
    List<InventoryCountingSerial> Serials);

public record InventoryCountingBatch(
    string ItemCode,
    string BatchCode,
    double Qty,
    DateTime ExpireDate,
    DateTime ManfectureDate,
    DateTime AdmissionDate,
    string LotNo,
    int BinEntry);

public record InventoryCountingSerial(
    string ItemCode,
    string SerialCode,
    int SystemSerialNumber,
    string MfrNo,
    DateTime MfrDate,
    DateTime ExpDate,
    string Location,
    DateTime ReceiptDate,
    int BinEntry);
