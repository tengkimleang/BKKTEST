using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;

namespace Tri_Wall.Shared.Views.GoodReceptPo;

public partial class ListGoodReceiptPo
{

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public Dictionary<string, object> Content { get; set; } = default!;
    
    private IEnumerable<TotalItemCount> TotalItemCounts => Content["totalItemCount"] as IEnumerable<TotalItemCount> ?? new List<TotalItemCount>();
    private bool IsDelete => Content.ContainsKey("isDelete");
    private bool IsSelete => Content.ContainsKey("isSelete");
    
    private Func<int, Task<ObservableCollection<GetListData>>> GetListData => Content["getData"] as Func<int, Task<ObservableCollection<GetListData>>>?? default!;

    private Func<string,Task> OnSeleteAsync => Content["onSelete"] as Func<string,Task> ?? default!;
    
    private Func<string,Task> OnDeleteAsync => Content["onDelete"] as Func<string,Task> ?? default!;

    private string? dataGrid = "width: 1240px;height:300px;";
    
    private IEnumerable<GetListData> _goodReceiptPoHeaders= new List<GetListData>();
    
    PaginationState pagination = new();
    
    protected override async Task OnInitializedAsync()
    {
        await pagination.SetTotalItemCountAsync(Convert.ToInt32(TotalItemCounts.FirstOrDefault()?.AllItem)).ConfigureAwait(false);
        await pagination.SetCurrentPageIndexAsync(0).ConfigureAwait(false);
        _goodReceiptPoHeaders =await GetListData(0);
    }

    private async Task LoadData(int page)
    {
        _goodReceiptPoHeaders =await GetListData(page);
    }
    
    private async Task SelectAsync(string docNum)
    {
        await Dialog.CloseAsync();
        await OnSeleteAsync(docNum);
    }
    
    private void UpdateGridSize(GridItemSize size)
    {
        dataGrid = size == GridItemSize.Xs ? "width: 1200px;height:205px" : "width: 100%;height:405px";
    }
}