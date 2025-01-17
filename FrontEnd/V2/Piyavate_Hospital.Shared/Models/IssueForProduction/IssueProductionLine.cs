﻿
using Piyavate_Hospital.Shared.Models.Gets;

namespace Piyavate_Hospital.Shared.Models.IssueForProduction;

public class IssueProductionLine
{
    public string DocNum { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public int BaseLineNum { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public double Qty { get; set; }
    public double QtyRequire { get; set; }
    public double Price { get; set; }
    public string WhsCode { get; set; } = string.Empty;
    public string UomName { get; set; } = string.Empty;
    public string? ManageItem { get; set; }
    public List<BatchIssueProduction> Batches { get; set; }= new();
    public List<SerialIssueProduction> Serials { get; set; }= new();
}

public class BatchIssueProduction
{
    public string BatchCode { get; set; } = string.Empty;
    public double Qty { get; set; }
    public DateTime? ExpDate { get; set; } = DateTime.Today;
    public DateTime? ManfectureDate { get; set; } = DateTime.Today;
    public DateTime? AdmissionDate { get; set; } = DateTime.Today;
    public string LotNo { get; set; } = string.Empty;
    public double QtyAvailable { get; set; }
    public IEnumerable<GetBatchOrSerial> OnSelectedBatchOrSerial { get; set; } = Array.Empty<GetBatchOrSerial>();
}

public class SerialIssueProduction
{
    public string SerialCode { get; set; } = string.Empty;
    public int Qty { get; set; } = 1;
    public string MfrNo { get; set; } = string.Empty;
    public DateTime? MfrDate { get; set; } = DateTime.Today;
    public DateTime? ExpDate { get; set; } = DateTime.Today;
    public IEnumerable<GetBatchOrSerial> OnSelectedBatchOrSerial { get; set; } = Array.Empty<GetBatchOrSerial>();
}
