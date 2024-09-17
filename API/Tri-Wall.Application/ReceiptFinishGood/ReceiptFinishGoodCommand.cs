using MediatR;
using ErrorOr;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.ReceiptFinishGood;

public record ReceiptFinishGoodCommand(
    int Series,
    DateTime DocDate,
    string Remarks,
    List<ReceiptFinishGoodLine> Lines,
    bool IsDraft = false
) : IRequest<ErrorOr<PostResponse>>;

public record ReceiptFinishGoodLine(
    string ItemCode,
    int DocNum,
    int BaseLineNum,
    double Qty,
    string WhsCode,
    string ManageItem,
    int TransactionType,
    List<ReceiptFinishGoodBatch>? Batches,
    List<ReceiptFinishGoodSerial>? Serials
);

public record ReceiptFinishGoodBatch(
    string BatchCode,
    double Qty,
    DateTime? ExpDate,
    DateTime? ManfectureDate,
    DateTime? AdmissionDate,
    string LotNo);

public record ReceiptFinishGoodSerial(
    string SerialCode,
    double Qty,
    string MfrNo,
    DateTime? MfrDate,
    DateTime? ExpDate);