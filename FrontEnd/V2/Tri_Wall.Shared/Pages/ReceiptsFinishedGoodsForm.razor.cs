using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.VisualBasic;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.ReceiptFinishGood;
using Tri_Wall.Shared.Models.ReturnComponentProduction;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;
using Tri_Wall.Shared.Views.ReceiptFromProduction;

namespace Tri_Wall.Shared.Pages;

public partial class ReceiptsFinishedGoodsForm
{
    private bool _isXs;
    private bool _init;
    private void UpdateGridSize(GridItemSize size)
    {
        _init=true;
        _isXs = size == GridItemSize.Xs;
    }
}