using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.DeliveryOrder.MobileAppScreen;

public partial class ListSearch
{
    int refreshcount = 0;
    int count = 10;
    private string? _searchValue;
    private ObservableCollection<GetListData> searchData = new();

    protected override void OnInitialized()
    {
        ComponentAttribute.Title = "List Search";
        ComponentAttribute.Path = "/deliveryorder";
        ComponentAttribute.IsBackButton = true;
    }

    private void OnSearch()
    {
        // if (!string.IsNullOrWhiteSpace(_searchValue))
        // {
        //     // You can also call an API here if the list is not local.
        //     var results = searchData
        //         .Where(str => str.Contains(_searchValue, StringComparison.OrdinalIgnoreCase))
        //         .Select(str => str)
        //         .ToList();

        //     _searchResults = results.Any() ? results : DefaultResults();
        // }
        // else
        // {
        //     _searchResults = DefaultResults();
        // }
    }

    public async Task<bool> OnRefreshAsync()
    {
        Console.WriteLine(Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem));
        Console.WriteLine(count);
        if(count == Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem??"0"))
        {
            Console.WriteLine("Hello False");
            return false;
        }
        Console.WriteLine(Convert.ToInt32(ViewModel.TotalItemCount.FirstOrDefault()?.AllItem));
        Console.WriteLine(searchData.Count);
        await ViewModel.GetGoodReceiptPoCommand.ExecuteAsync(refreshcount.ToString()).ConfigureAwait(false);
        foreach (var item in ViewModel.GetListData)
        {
            searchData.Add(item);
        }
        refreshcount++;
        count = + searchData.Count;
        return true;
    }
}