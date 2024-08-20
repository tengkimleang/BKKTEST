using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;
public static class ErrorHandlingHelper
{
    public static async Task ExecuteWithHandlingAsync(Func<Task> action, PostResponse postResponse, IToastService toastService)
    {
        try
        {
            await action();
        }
        catch (ApiException ex)
        {
            if (postResponse.ErrorCode == "")
            {
                toastService.ShowError(ex.Content ?? "");
            }
            else
            {
                toastService.ShowError(postResponse.ErrorMsg);
            }
        }
    }

}
