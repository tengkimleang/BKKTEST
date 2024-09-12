
using System.Collections.ObjectModel;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.DeliveryOrder.MobileAppScreen.List;

public partial class ListSearchSalesOrder
{
    private int _refreshCount;
    private int _count;
    private string? _searchValue;
    private readonly ObservableCollection<GetListData> _scrollingData = new();
    public bool IsViewDetail;
    protected override async void OnInitialized()
    {
        StateHasChanged();
        ComponentAttribute.Title = "List Search";
        ComponentAttribute.Path = "/deliveryorder";
        ComponentAttribute.IsBackButton = true;
        await OnRefreshAsync();
        IsViewDetail = false;
    }
    private void UpdateGridSize(GridItemSize size)
    {
        if (size != GridItemSize.Xs)
        {
            NavigationManager.NavigateTo("deliveryorder");
        }
    }
    private async Task OnSearch()
    {
        if (!string.IsNullOrWhiteSpace(_searchValue))
        {
            _scrollingData.Clear();
            _count = 0;
            _refreshCount = 0;
            var dataSearch = new Dictionary<string, object> { { "docNum", _searchValue },{"dateFrom",""},{"dateTo",""} };
            await ViewModel.GetGoodReceiptPoBySearchCommand.ExecuteAsync(dataSearch).ConfigureAwait(false);
            foreach (var item in ViewModel.GetListData)
            {
                _scrollingData.Add(item);
            }
            StateHasChanged();
        }
        else
        {
            await OnRefreshAsync().ConfigureAwait(false);
        }
    }

    public async Task<bool> OnRefreshAsync()
    {
        if(Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem??"0")<=_count)
        {
            return false;
        }
        await ViewModel.GetPurchaseOrderCommand.ExecuteAsync(_refreshCount.ToString()).ConfigureAwait(false);
        foreach (var item in ViewModel.GetListData)
        {
            _scrollingData.Add(item);
        }
        _refreshCount++;
        _count = + _scrollingData.Count;
        StateHasChanged();
        return true;
    }

    private Task OnClickCopy(string docEntry)
    {
        NavigationManager.NavigateTo($"/addDeliveryOrderMobile/{docEntry}");
        return Task.CompletedTask;
    }
}