namespace Tri_Wall.Shared.Models.ProductionProcess;

public class ProductionProcessHeader
{
    public List<ProcessProductionLine> Data { get; set; }= new List<ProcessProductionLine>();
}
public class ProcessProductionLine
{
    public int ProductionNo { get; set; }
    public string ProcessStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}