using System.Collections.ObjectModel;

namespace Tri_Wall.Shared.Models.ProductionProcess;

public class ProductionProcessHeader
{
    public ObservableCollection<ProcessProductionLine> Data { get; set; }= new();
}
