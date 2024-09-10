using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.DeliveryOrder.MobileAppScreen.List;

public partial class ListSearch
{
    int refreshcount = 0;
    int count = 0;
    private string? _searchValue;
    private ObservableCollection<GetListData> scrollingData = new();
    private bool _isViewDetail = false;
    protected override async void OnInitialized()
    {
        StateHasChanged();
        ComponentAttribute.Title = "List Search";
        ComponentAttribute.Path = "/deliveryorder";
        ComponentAttribute.IsBackButton = true;
        await OnRefreshAsync();
        _isViewDetail = false;
    }
    
    private async Task OnSearch()
    {
        if (!string.IsNullOrWhiteSpace(_searchValue))
        {
            scrollingData.Clear();
            count = 0;
            refreshcount = 0;
            var dataSearch = new Dictionary<string, object> { { "docNum", _searchValue },{"dateFrom",""},{"dateTo",""} };
            await ViewModel.GetGoodReceiptPoBySearchCommand.ExecuteAsync(dataSearch).ConfigureAwait(false);
            foreach (var item in ViewModel.GetListData)
            {
                scrollingData.Add(item);
            }
            StateHasChanged();
            // tmpData= new ObservableCollection<GetListData>(scrollingData);
            // // You can also call an API here if the list is not local.
            // var results = ViewModel.GetListData
            //     .Where(item => item.DocumentNumber.Contains(_searchValue, StringComparison.OrdinalIgnoreCase) 
            //                    || item.VendorCode.Contains(_searchValue, StringComparison.OrdinalIgnoreCase) 
            //                    || item.DocDate.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
            //                    || item.Remarks.Contains(_searchValue, StringComparison.OrdinalIgnoreCase))
            //     .ToList();
            // scrollingData.Clear();
            // foreach (var result in results)
            // {
            //     scrollingData.Add(result);
            // }
        }
        else
        {
            await OnRefreshAsync().ConfigureAwait(false);
        }
    }

    public async Task<bool> OnRefreshAsync()
    {
        if(Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem??"0")<=count)
        {
            return false;
        }
        Console.WriteLine(Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem));
        Console.WriteLine(scrollingData.Count);
        await ViewModel.GetGoodReceiptPoCommand.ExecuteAsync(refreshcount.ToString()).ConfigureAwait(false);
        foreach (var item in ViewModel.GetListData)
        {
            scrollingData.Add(item);
        }
        refreshcount++;
        count = + scrollingData.Count;
        StateHasChanged();
        return true;
    }

    private async Task OnClickCopy(string docEntry)
    {
        await ViewModel.GetGoodReceiptPoHeaderDeatialByDocNumCommand.ExecuteAsync(docEntry).ConfigureAwait(false);
        _isViewDetail=true;
    }
    
    private Task OnViewDetail()
    {
        _isViewDetail = false;
        StateHasChanged();
        return Task.CompletedTask;
    }
    
}