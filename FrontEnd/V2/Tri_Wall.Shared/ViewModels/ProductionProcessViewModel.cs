using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.ProductionProcess;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class ProductionProcessViewModel(ApiService apiService) : ViewModelBase
{
    #region Data Member
    [ObservableProperty] ProductionProcessHeader _productionProcessHeader = new();
    [ObservableProperty] ObservableCollection<GetProductionOrder> _getProductionOrder = new();
    [ObservableProperty] Boolean _isView;
    [ObservableProperty] PostResponse _postResponses = new();
    [ObservableProperty] ProcessProductionLine _processProductionLine = new();
    #endregion
    
    public override async Task Loaded()
    {
        GetProductionOrder = await CheckingValueT(GetProductionOrder, async () =>
            (await apiService.GetProductionOrders("GetForReceiptProduction")).Data ?? new());
        IsView = true;
    }
    [RelayCommand]
    async Task Submit()
    {
        PostResponses = await apiService.PostProductionProcess(ProductionProcessHeader);
    }
}