
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;

namespace Tri_Wall.Shared.Views.ReturnRequest.MobileAppScreen.View;

public partial class ViewItemDetailReturnRequest
{
    [Parameter] public GoodReceiptPoLineByDocNum ItemDetail { get; set; } = new();
    [Parameter] public List<GetBatchOrSerial> GetBatchOrSerials { get; set; } = new();
    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("goodreturn");
        }
    }
    [Parameter] public Func<Task> IsViewDetailBack { get; set; }=default!;
}