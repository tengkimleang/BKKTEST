

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;

namespace Tri_Wall.Shared.Views.Shared.Component;

public partial class PrintLayout
{
    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public Dictionary<string, object> Content { get; set; } = default!;

    private ObservableCollection<GetLayout> GetListData => Content["getLayout"] as ObservableCollection<GetLayout> ?? default!;
    

}