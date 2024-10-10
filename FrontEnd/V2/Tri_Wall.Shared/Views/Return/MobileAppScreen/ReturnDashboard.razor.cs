using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.Return.MobileAppScreen;

public partial class ReturnDashboard 
{
    protected override void OnInitialized()
    {
        ComponentAttribute.Title = "Return";
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
        NavigationManager.NavigateTo("/ListReturn");
    }

    private void OnClickAddDeliveryOrderMobile()
    {
        NavigationManager.NavigateTo("/AddReturnMobile/");
    }
    private void OnClickListSearchSalesOrder()
    {
        NavigationManager.NavigateTo("/ListSearchDeliveryOrderByReturn");
    }
}