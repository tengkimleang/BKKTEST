

using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages;

public partial class DeliveryOrderForm
{
    private bool isXs = false;
    bool visible = false;
    protected void OnCloseOverlay() => visible = true;
    private void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            isXs = true;
        }
        else
        {
            isXs = false;
        }
    }

}