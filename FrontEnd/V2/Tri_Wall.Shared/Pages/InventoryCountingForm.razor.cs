using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryCounting;
using Tri_Wall.Shared.Models.IssueForProduction;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.InventoryCounting;

namespace Tri_Wall.Shared.Pages;

public partial class InventoryCountingForm
{
    private bool _isXs;
    private bool _init;
    private void UpdateGridSize(GridItemSize size)
    {
        _init=true;
        _isXs = size == GridItemSize.Xs;
    }
}