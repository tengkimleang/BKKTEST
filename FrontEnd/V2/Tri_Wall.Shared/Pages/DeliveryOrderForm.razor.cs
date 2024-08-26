

using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages;

public partial class DeliveryOrderForm
{
    bool isView = false;
    private bool isXs = false;
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