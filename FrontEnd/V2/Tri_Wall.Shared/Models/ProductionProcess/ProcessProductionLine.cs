﻿namespace Tri_Wall.Shared.Models.ProductionProcess;

public class ProcessProductionLine
{
    public string DocNum { get; set; } = string.Empty;
    public int ProductionNo { get; set; }
    public string ProcessStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}