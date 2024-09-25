using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.InventoryTransfer;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.InventoryTransfer;

namespace Tri_Wall.Shared.Pages;

public partial class InventoryTransfer
{
    private bool _isXs;
    private bool _init;
    private void UpdateGridSize(GridItemSize size)
    {
        _init=true;
        _isXs = size == GridItemSize.Xs;
    }
}
