
using System.ComponentModel.DataAnnotations;

namespace Tri_Wall.Shared.Models.GoodReceiptPo;

public class GoodReceiptPoHeader
{
    public string VendorCode { get; set; } = string.Empty;
    public string ContactPersonCode { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public DateTime? DocDate { get; set; }
    public DateTime? TaxDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public bool IsDraft { get; set; }
    public List<GoodReceiptPoLine>? Lines { get; set; }
}
