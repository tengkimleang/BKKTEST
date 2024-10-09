
using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages.GoodReturn;

public partial class GoodReturn
{
    private bool _isXs = false;
    bool _visible = false;
    private bool _init = false;
    protected void OnCloseOverlay() => _visible = true;
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