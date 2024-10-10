using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages.Return;

public partial class Return
{
    private bool _isXs;
    private bool _init;
    private void UpdateGridSize(GridItemSize size)
    {
        _init=true;
        if (size == GridItemSize.Xs)
        {
            _isXs = true;
        }
        else
        {
            _isXs = false;
        }
    }
}