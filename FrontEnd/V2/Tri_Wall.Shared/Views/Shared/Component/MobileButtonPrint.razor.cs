
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.Shared.Component;

public partial class MobileButtonPrint
{
    [Parameter] public ObservableCollection<GetLayout> GetLayouts { get; set; } = new();
    [Parameter] public string DocEntry { get; set; } = string.Empty;
    private async Task HandleOnMenuChanged(MenuChangeEventArgs args)
    {
        await JsRuntime.InvokeVoidAsync("window.open", $"{ApiConstant.ApiUrl}/layoutEndpoint?docEntry={DocEntry}&layoutCode={args.Id}");
    }
}