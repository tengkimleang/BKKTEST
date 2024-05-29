using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Pages;

namespace Tri_Wall.Shared.Views.GoodReceptPo;

public partial class ViewGoodReceiptPoForm
{
    //[Parameter]
    //public GoodReceiptPoForm ViewModel { get; set; } = default!;
    [Parameter]
    public Func<Task> AddNew { get; set; } = default!;
    [Parameter]
    public Func<Task> Search { get; set; } = default!;
    string dataGrid = "width: 100%;height:405px";
    void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            dataGrid = "width: 700px;height:205px";
        }
        else
        {
            dataGrid = "width: 100%;height:405px";
        }
    }
}
