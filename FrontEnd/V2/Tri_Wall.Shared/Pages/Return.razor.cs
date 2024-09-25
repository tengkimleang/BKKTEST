using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Tri_Wall.Shared.Models.DeliveryOrder;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Views.Return;
using Tri_Wall.Shared.Views.GoodReceptPo;
using System.Text.Json;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Pages;

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