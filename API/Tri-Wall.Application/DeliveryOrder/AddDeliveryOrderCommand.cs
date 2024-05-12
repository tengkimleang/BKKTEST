using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.DeliveryOrder;

public record AddDeliveryOrderCommand : IRequest<ErrorOr<PostResponse>>
{
    public string CardCode { get; set; } = null!;
    public int ContactPersonCode { get; set; }
    public string NumAtCard { get; set; } = "";
    public int Series { get; set; }
    public DateTime DocDate { get; set; } = DateTime.Today;
    public DateTime TaxDate { get; set; } = DateTime.Today;
    public string Remarks { get; set; } = "";
    public List<DeliveryItemLine> Lines { get; set; } = null!;
}
public record DeliveryItemLine
{
    public int BaseDocEntry { get; set; }
    public int BaseLineNumber { get; set; }
    public string ItemCode { get; set; } = null!;
    public double Qty { get; set; }
    public double Price { get; set; }
    public string? VatCode { get; set; }
    public string? WarehouseCode { get; set; }
    public string? ManageItem { get; set; }
    public string? LineStatus { get; set; }
    public List<DeliveryBatch>? Batches { get; set; }
    public List<DeliverySerial>? Serials { get; set; }

}
public record DeliveryBatch
{
    public string ItemCode { get; set; } = null!;
    public string BatchOrSerialCode { get; set; } = null!;
    public double Qty { get; set; }
    public string ExpDate { get; set; } = null!;
    public string LotNumber { get; set; } = null!;
    public string AdmissionDate { get; set; } = null!;
    public string SysNumber { get; set; } = null!;
}
public record DeliverySerial
{
    public string ItemCode { get; set; } = null!;
    public string BatchOrSerialCode { get; set; } = null!;
    public double Qty { get; set; }
    public string ExpDate { get; set; } = null!;
    public string LotNumber { get; set; } = null!;
    public string AdmissionDate { get; set; } = null!;
    public string SysNumber { get; set; } = null!;
}