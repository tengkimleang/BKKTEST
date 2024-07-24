
using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.GoodReturn;

public record AddGoodReturnCommand(
    string CustomerCode,
    int Series,
    DateTime DocDate,
    DateTime TaxDate,
    List<GoodReturnLine> Lines,
    string Remarks="",
    string NumAtCard ="",
    int ContactPersonCode=0,
    bool IsDraft=false) : IRequest<ErrorOr<PostResponse>>;

public record GoodReturnLine(
    string ItemCode,
    double Qty,
    double Price,
    string VatCode,
    string WarehouseCode,
    string ManageItem,
    int BaseEntry,
    int BaseLine,
    List<BatchGoodReturn> Batches,
    List<SerialGoodReturn> Serials);

public record BatchGoodReturn(
    string BatchCode,
    double Qty,
    DateTime ExpireDate,
    DateTime ManfectureDate,
    DateTime AdmissionDate,
    string LotNo);

public record SerialGoodReturn(
    string SerialCode,
    double Qty,
    string MfrNo,
    DateTime MfrDate,
    DateTime ExpDate);