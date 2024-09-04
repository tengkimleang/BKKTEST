
using Microsoft.AspNetCore.Components;

namespace Tri_Wall.Shared.Services;

public static class ComponentAttribute
{
    public static string Path = "";
    public static string Title = "DeliveryOrder";
    public static bool IsBackButton;
    public static void Navigation(NavigationManager navigationManager)
    {
        navigationManager.NavigateTo(Path);
    }
    
}