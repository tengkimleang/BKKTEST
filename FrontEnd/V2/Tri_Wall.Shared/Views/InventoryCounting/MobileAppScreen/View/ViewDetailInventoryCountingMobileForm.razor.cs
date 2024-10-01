
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;

namespace Tri_Wall.Shared.Views.InventoryCounting.MobileAppScreen.View;

public partial class ViewDetailInventoryCountingMobileForm
{
    [Parameter]
    public ObservableCollection<GetDetailInventoryCountingHeaderByDocNum> GetDetailInventoryCountingHeaderByDocNum { get; set; } =
        new();
    [Parameter] public Func<Task> IsViewDetail { get; set; }=default!;
    [Parameter] public ObservableCollection<GetDetailInventoryCountingLineByDocNum> GetDetailInventoryCountingLineByDocNum { get; set; } = new();
    [Parameter] public ObservableCollection<GetBatchOrSerial> GetBatchOrSerials { get; set; } = new();
    
    bool _isShowBatchSerial=false;
    private string _itemCode=string.Empty;
    private string _lineNum=string.Empty;
    
    private Task OnViewItemDetail()
    {
        _isShowBatchSerial = false;
        StateHasChanged();
        return Task.CompletedTask;
    }
    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("deliveryorder");
        }
    }
    void ShowItemDetail(string itemCode, string lineNum)
    {
        this._itemCode = itemCode;
        this._lineNum = lineNum;
        _isShowBatchSerial = true;
        StateHasChanged();
    }
    private async Task HandleOnMenuChanged(MenuChangeEventArgs args)
    {
        
    }
}