
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Views.ReturnComponent;

public partial class ReturnComponentMobile
{
    protected override void OnInitialized()
    {
        ComponentAttribute.Title = "Inventory Transfer";
        ComponentAttribute.Path = "/home";
        ComponentAttribute.IsBackButton = true;
    }

    private void OnFloatButtonClick()
    {
        // Handle the float button click event
    }

    void UpdateGridSize(GridItemSize size)
    {
        // if (size == GridItemSize.Xs)
        // {
        //     stringDisplay = "";
        //     dataGrid = "width: 1600px;height:205px";
        //     fromWord = "";
        //     saveWord = "S-";
        // }
        // else
        // {
        //     stringDisplay = "Delivery Order";
        //     fromWord = "From";
        //     saveWord = "Save";
        //     dataGrid = "width: 1600px;height:405px";
        // }
    }

    private void OnClickList()
    {
        NavigationManager.NavigateTo("/ListSearchReturnComponentMobile");
    }

    private void OnClickAddDeliveryOrderMobile()
    {
        NavigationManager.NavigateTo("/AddReturnComponentMobile");
    }

    private void OnClickListSearchSalesOrder()
    {
        NavigationManager.NavigateTo("/ListSearchSalesOrder");
    }
}