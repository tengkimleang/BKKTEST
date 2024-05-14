using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.ConstrainedExecution;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class GoodReceptPoViewModel(ApiService apiService) : ViewModelBase
{
    private readonly ApiService apiService = apiService;
    [ObservableProperty]
    IEnumerable<string> _selectedItems = Array.Empty<string>();

    [ObservableProperty]
    ObservableCollection<Series> _series=new();

    public override async Task OnInitializedAsync()
    {
        Series =await CheckingValueT(Series, async () => 
            new ObservableCollection<Series>(
                (await apiService.GetSeries("20")).Data?? new()));
    }
    async Task<T> CheckingValueT<T>(T t, Func<Task<T>> func) where T : ICollection
    {
        if (t == null || t.Count == 0) return await func();
        return t;
    }
}
