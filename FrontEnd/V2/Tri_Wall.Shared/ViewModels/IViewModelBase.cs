
using System.ComponentModel;

namespace Tri_Wall.Shared.ViewModels;

public interface IViewModelBase : INotifyPropertyChanged
{
    Task OnInitializedAsync();
    Task Loaded();
}
