
using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages;

public partial class ReturnRequest
{
    private bool _isXs;
    bool _visible;
    private bool _init;
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