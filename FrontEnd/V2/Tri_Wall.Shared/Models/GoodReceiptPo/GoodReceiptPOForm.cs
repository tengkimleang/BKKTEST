
using System.ComponentModel.DataAnnotations;

namespace Tri_Wall.Shared.Models.GoodReceiptPo;

public class GoodReceiptPOForm
{
    public string CardCode { get; set; }= string.Empty;
    public string ContactPersonCode { get; set; }= string.Empty;
    [Required(ErrorMessage ="Can not be empty")]
    public List<GoodReceiptPOLine>? Lines { get; set; }
}

public class GoodReceiptPOLine
{
    public string ItemCode { get; set; } = string.Empty;
}
