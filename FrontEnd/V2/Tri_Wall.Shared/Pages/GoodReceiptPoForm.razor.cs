using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Views.GoodReceptPo;

namespace Tri_Wall.Shared.Pages;

public partial class GoodReceiptPoForm
{
    [Inject]
    public IValidator<GoodReceiptPoHeader>? Validator { get; init; }
    
    private string stringDisplay = "Recept";
    
    string? dataGrid = "width: 100%;height:405px";
    bool isView=false;
    protected void OnCloseOverlay() => visible = true;
    
    IEnumerable<Vendors> selectedVendor = Array.Empty<Vendors>();

    bool visible = false;
    
    async Task OpenDialogAsync(GoodReceiptPoLine goodReceiptPoLine)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "item", ViewModel.Items },
            { "taxPurchase", ViewModel.TaxPurchases },
            { "warehouse", ViewModel.Warehouses },
            { "line", goodReceiptPoLine }
        };

        var dialog = await DialogService!.ShowDialogAsync<DialogAddLine>(dictionary, new DialogParameters
        {
            Title = (goodReceiptPoLine == null) ? "Add Line" : "Update Line",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);

        var result = await dialog.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is Dictionary<string, object> data)
        {
            if(ViewModel.GoodReceiptPoForm.Lines==null) ViewModel.GoodReceiptPoForm.Lines = new List<GoodReceiptPoLine>();
            if(data["data"] is GoodReceiptPoLine receiptPoLine)
            {
                if (receiptPoLine.LineNum == 0)
                {
                    receiptPoLine.LineNum = ViewModel.GoodReceiptPoForm.Lines.Count + 1;
                    ViewModel.GoodReceiptPoForm.Lines.Add(receiptPoLine);
                }
                else
                {
                    var index = ViewModel.GoodReceiptPoForm.Lines.FindIndex(i => i.LineNum == receiptPoLine.LineNum);
                    ViewModel.GoodReceiptPoForm.Lines[index] = receiptPoLine;
                }
            }
        }
    }

    private void OnSearch(OptionsSearchEventArgs<Vendors> e)
    {
        e.Items = ViewModel.Vendors.Where(i => i.VendorCode.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                            i.VendorName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(i => i.VendorCode);
    }

    void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            stringDisplay = "";
            dataGrid = "width: 700px;height:205px";
        }
        else
        {
            stringDisplay = "Recept";
            dataGrid = "width: 100%;height:405px";
        }
    }
    private void DeleteLine(int index)
    {
        ViewModel.GoodReceiptPoForm.Lines!.RemoveAt(index);
    }
    async Task OnSaveTransaction()
    {
        ViewModel.GoodReceiptPoForm.VendorCode = selectedVendor.FirstOrDefault()?.VendorCode ?? "";
        ViewModel.GoodReceiptPoForm.DocDate = DateTime.Now;
        var result = await Validator!.ValidateAsync(ViewModel.GoodReceiptPoForm).ConfigureAwait(false);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }
            return;
        }
        try
        {
            visible = true;
            
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);
            
            if(ViewModel.PostResponses.ErrorCode=="")
                ToastService.ShowSuccess("Success");
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
            visible = false;
        }
        catch (Exception ex)
        {
            string errorMessage = ex.InnerException?.Message ?? ex.Message;
            ToastService!.ShowError(errorMessage);
            visible = false;
        }
    }
    Task OnSeleted(string e)
    {
        
        Console.WriteLine(e);
        isView=true;
        StateHasChanged();
        return Task.CompletedTask;
    }
    
    Task OnDelete(string e)
    {
        Console.WriteLine(e);
        return Task.CompletedTask;
    }
    Task OnView()
    {
        isView = false;
        StateHasChanged();
        return Task.CompletedTask;
    }
    async Task<ObservableCollection<GetListData>> GetListData(int p)
    {
        await ViewModel.GetGoodReceiptPoCommand.ExecuteAsync(p.ToString());
        return ViewModel.GetListData;
    }
    async Task OpenListDataAsyncAsync()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "totalItemCount", ViewModel.TotalItemCount },
            { "getData", new Func<int, Task<ObservableCollection<GetListData>>>(GetListData) },
            //{ "isDelete", true },
            { "isSelete", true },
            {"onSelete",new Func<string,Task>(OnSeleted)},
            //{"onDelete",new Func<string,Task>(OnDelete)},
        };
        await DialogService!.ShowDialogAsync<ListGoodReceiptPo>(dictionary, new DialogParameters
        {
            Title = "List Good receipt PO",
            PreventDismissOnOverlayClick = true,
            PreventScroll = false,
            Width = "80%",
            Height = "80%"
        }).ConfigureAwait(false);
    }
    
}
