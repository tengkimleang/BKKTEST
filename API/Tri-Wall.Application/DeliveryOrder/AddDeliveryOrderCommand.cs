using ErrorOr;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.DeliveryOrder;

public class AddDeliveryOrderCommand:IRequest<ErrorOr<PostResponse>>
{
    [Required(ErrorMessage = "CardCode is Require")]
    public string CardCode { get; set; } = null!;
    public int ContactPersonCode { get; set; }
    public string NumAtCard { get; set; } = "";
    [Required(ErrorMessage = "Series is Require")]
    public int Series { get; set; }
    public DateTime DocDate { get; set; } = DateTime.Today;
    public DateTime TaxDate { get; set; } = DateTime.Today;
    public string Remarks { get; set; } = "";
    public int BranchID { get; set; }
    public string ARDocNum { get; set; } = string.Empty;
    public int DocEntry { get; set; }
    public List<DeliveryItemLine>? Lines { get; set; }
}
public class DeliveryItemLine
{
    public int BaseDocEntry { get; set; }
    public int BaseLineNumber { get; set; }
    public string ItemCode { get; set; } = null!;
    public double Qty { get; set; }
    public double Price { get; set; }
    public string VatCode { get; set; } = null!;
    public string WarehouseCode { get; set; } = null!;
    public string ManageItem { get; set; } = null!;
    public string LineStatus { get; set; } = null!;
    public List<DeliveryBatch> Batches { get; set; } = null!;
    public List<DeliverySerial> Serials { get; set; } = null!;

}
public class DeliveryBatch
{
    public string ItemCode { get; set; } = null!;
    public string BatchOrSerialCode { get; set; } = null!;
    public double Qty { get; set; }
    public string ExpDate { get; set; } = null!;
    public string LotNumber { get; set; } = null!;
    public string AdmissionDate { get; set; } = null!;
    public string SysNumber { get; set; } = null!;
}
public class DeliverySerial
{
    public string ItemCode { get; set; } = null!;
    public string BatchOrSerialCode { get; set; } = null!;
    public double Qty { get; set; }
    public string ExpDate { get; set; } = null!;
    public string LotNumber { get; set; } = null!;
    public string AdmissionDate { get; set; } = null!;
    public string SysNumber { get; set; } = null!;
}