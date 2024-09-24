
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.ReturnRequest;

public partial class ReturnRequestMobile
{
    protected override void OnInitialized()
    {
        ComponentAttribute.Title = "Return Request";
        ComponentAttribute.Path = "/home";
        ComponentAttribute.IsBackButton = true;
    }
    private void OnFloatButtonClick()
    {
        // Handle the float button click event
    }
    void UpdateGridSize(GridItemSize size)
    {
        
    }

    private void OnClickList()
    {
        NavigationManager.NavigateTo("/ListSearchReturnRequest");
    }

    private void OnClickAddDeliveryOrderMobile()
    {
        NavigationManager.NavigateTo("/AddReturnRequestMobile/");
    }
    private void OnClickListSearchSalesOrder()
    {
        NavigationManager.NavigateTo("/ListSearchDeliveryOrderByReturnRequest");
    }
}