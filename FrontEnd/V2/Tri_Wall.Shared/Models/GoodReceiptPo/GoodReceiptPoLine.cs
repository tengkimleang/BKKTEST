﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Shared.Models.GoodReceiptPo;

public class GoodReceiptPoLine
{
    public string ItemCode { get; set; } = string.Empty;
    public double? Qty { get; set; }
    public double? Price { get; set; }
    public string? VatCode { get; set; }
    public string? WarehouseCode { get; set; }
    public string? ManageItem { get; set; }
    public List<BatchReceiptPo>? Batches { get; set; }
    public List<SerialReceiptPo>? Serials { get; set; }
}

public class BatchReceiptPo
{
    public string BatchCode { get; set; } = string.Empty;
    public double Qty { get; set; }
    public DateTime? ExpDate { get; set; }
    public DateTime? ManfectureDate { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public string LotNo { get; set; } = string.Empty;
}

public class SerialReceiptPo
{
    public string SerialCode { get; set; } = string.Empty;
    public double Qty { get; set; } = 1;
    public string MfrNo { get; set; } = string.Empty;
    public DateTime? MfrDate { get; set; }
    public DateTime? ExpDate { get; set; }
}
