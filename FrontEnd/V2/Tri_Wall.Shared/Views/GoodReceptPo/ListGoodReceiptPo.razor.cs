using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;

namespace Tri_Wall.Shared.Views.GoodReceptPo;

public partial class ListGoodReceiptPo
{

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public Dictionary<string, object> Content { get; set; } = default!;
    private IEnumerable<TotalItemCount> TotalItemCounts => Content["totalItemCount"] as IEnumerable<TotalItemCount> ?? new List<TotalItemCount>();
    
    private Func<int, Task<ObservableCollection<GoodReceiptPoHeader>>> GetListData => Content["getData"] as Func<int, Task<ObservableCollection<GoodReceiptPoHeader>>>;
    
    string? dataGrid = "width: 100%;";
    
    private IEnumerable<GoodReceiptPoHeader> _goodReceiptPoHeaders= new List<GoodReceiptPoHeader>();
    
    PaginationState pagination = new();
    protected override async Task OnInitializedAsync()
    {
        await pagination.SetTotalItemCountAsync(Convert.ToInt32(TotalItemCounts.FirstOrDefault()?.AllItem)).ConfigureAwait(false);
        await pagination.SetCurrentPageIndexAsync(0).ConfigureAwait(false);
    }

    private async Task LoadData(int page)
    {
        Console.WriteLine(page);
        _goodReceiptPoHeaders = new List<GoodReceiptPoHeader>();
        Console.WriteLine("Hello");
        _goodReceiptPoHeaders =await GetListData(page);
        // Call your API and update your data here
        // For example:
        // var data = await Api.GetData(pagination.CurrentPageIndex, pagination.PageSize);
        // Update your data source with the new data
    }
    private async Task SaveAsync()
    {
        await Dialog.CloseAsync(new Dictionary<string, object>
        {
            { "data", null },
            { "isUpdate", Content.ContainsKey("line") }
        });
    }

    private void UpdateGridSize(GridItemSize size)
    {
        dataGrid = size == GridItemSize.Xs ? "width: 1200px;height:205px" : "width: 100%;height:405px";
    }
}