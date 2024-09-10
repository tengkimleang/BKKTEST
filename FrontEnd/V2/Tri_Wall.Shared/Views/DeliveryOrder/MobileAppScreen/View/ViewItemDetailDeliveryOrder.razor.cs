
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;

namespace Tri_Wall.Shared.Views.DeliveryOrder.MobileAppScreen.View;

public partial class ViewItemDetailDeliveryOrder
{
    [Parameter] public GoodReceiptPoLineByDocNum ItemDetail { get; set; } = new();
    [Parameter] public List<GetBatchOrSerial> GetBatchOrSerials { get; set; } = new();

[Parameter] public Func<Task> IsViewDetailBack { get; set; }=default!;
}