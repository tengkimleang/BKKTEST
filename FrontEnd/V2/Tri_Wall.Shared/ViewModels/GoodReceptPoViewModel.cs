using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class GoodReceptPoViewModel : ViewModelBase
{
    private readonly ApiService apiService;
    private readonly IDialogService dialogService;
    public GoodReceptPoViewModel(ApiService apiService, IDialogService dialogService)
    {
        this.apiService = apiService;
        this.dialogService = dialogService;
    }

    [ObservableProperty]
    IEnumerable<string> _selectedItems = Array.Empty<string>();
    [ObservableProperty]
    ObservableCollection<Series> _series;
    //public override async Task Loaded()
    //{
    //    var result = await apiService.GetSeries("20");
    //    if (result.ErrorCode == "")
    //    {
    //        Series = new ObservableCollection<Series>(result.Data ?? new());
    //    }
    //    else
    //    {
    //        await dialogService.ShowErrorAsync(result.ErrorMessage);
    //    }
    //}
    [RelayCommand]
    async Task GetSeries()
    {
        var result = await apiService.GetSeries("20");
        if (result.ErrorCode == "")
        {
            Series = new ObservableCollection<Series>(result.Data ?? new());
        }
        else
        {
            await dialogService.ShowErrorAsync(result.ErrorMessage);
        }
    }
}
