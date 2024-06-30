using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.ReturnComponent;

public record AddReturnComponentCommand(
    int Series,
    DateTime DocDate,
    string Remarks,
    List<ReturnComponentLine> Lines,
    bool IsDraft = false
) : IRequest<ErrorOr<PostResponse>>;

public record ReturnComponentLine(
    int DocNum,
    int BaseLineNum,
    double Qty,
    string WhsCode,
    string ManageItem,
    List<Batch>? Batches,
    List<Serial>? Serials
);

public record Batch(
    string BatchCode,
    double Qty,
    DateTime? ExpDate,
    DateTime? ManfectureDate,
    DateTime? AdmissionDate,
    string LotNo);

public record Serial(
    string SerialCode,
    double Qty,
    string MfrNo,
    DateTime? MfrDate,
    DateTime? ExpDate);