using Microsoft.FluentUI.AspNetCore.Components;

namespace Tri_Wall.Shared.Pages.IssueForProduction;

public partial class IssueForProductionForm
{
    private bool _isXs;
    private bool _init;
    private void UpdateGridSize(GridItemSize size)
    {
        _init=true;
        _isXs = size == GridItemSize.Xs;
    }
}