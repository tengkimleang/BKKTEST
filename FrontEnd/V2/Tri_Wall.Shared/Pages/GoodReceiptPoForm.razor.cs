using System.Collections.ObjectModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;
using Tri_Wall.Shared.Views.GoodReceptPo;

namespace Tri_Wall.Shared.Pages;

public partial class GoodReceiptPoForm
{
    private bool isXs = false;
    bool visible = false;
    private bool init = false;
    protected void OnCloseOverlay() => visible = true;
    private void UpdateGridSize(GridItemSize size)
    {
        init=true;
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
