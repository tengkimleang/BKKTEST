﻿
using System.ComponentModel.DataAnnotations;

namespace Tri_Wall.Shared.Models.GoodReceiptPo;

public class GoodReceiptPOForm
{
    public string VendorCode { get; set; }= string.Empty;
    public string ContactPersonCode { get; set; }= string.Empty;
    public string VendorNo { get; set; }= string.Empty;
    public int Series { get; set; }
    public DateTime DocDate { get; set; }
    public DateTime TaxDate { get; set; }
    public string Remarks { get; set; }=string.Empty;
    public bool IsDraft { get; set; }
    [Required(ErrorMessage ="Can not be empty")]
    public List<GoodReceiptPOLine>? Lines { get; set; }
}

public class GoodReceiptPOLine
{
    [Required(ErrorMessage ="Item Code is Require")]
    public string ItemCode { get; set; } = string.Empty;
    [Required(ErrorMessage = "Qty is Bigger than zero")]
    public double Qty { get; set; }
    public double Price { get; set; }
    public string VatCode { get; set; } = string.Empty;
    [Required(ErrorMessage = "Warehouse code is require")]
    public string WarehouseCode { get; set; } = string.Empty;
    public string ManageItem { get; set; } = string.Empty;
    public List<BatchReceiptPO>? Batches { get; set; }
    public List<SerialReceiptPO>? Serials { get; set; }
}
public class BatchReceiptPO
{
    [Required(ErrorMessage ="BatchCode is require")]
    public string BatchCode { get; set; } = string.Empty;
    [Required(ErrorMessage ="Qty need to bigger than zero")]
    public double Qty { get; set; }
    public DateTime ExpDate { get; set; }
    public DateTime ManfectureDate { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string LotNo { get; set; }=string.Empty;
}

public class SerialReceiptPO
{
    [Required(ErrorMessage ="Serial is Require")]
    public string SerialCode { get; set; } = string.Empty;
    public double Qty { get; set; } = 1;
    public string MfrNo { get; set; } = string.Empty;
    public DateTime MfrDate { get; set; }
    public DateTime ExpDate { get; set; }
}