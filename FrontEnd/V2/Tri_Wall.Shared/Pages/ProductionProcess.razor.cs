﻿using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Tri_Wall.Shared.Models.Gets;
using Tri_Wall.Shared.Models.ProductionProcess;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Pages;

public partial class ProductionProcess
{
    [Inject] public IValidator<ProductionProcessHeader>? Validator { get; init; }

    private string _stringDisplay = "Production Process";
    string? _dataGrid = "width: 1600px;height:405px";
    protected void OnCloseOverlay() => _visible = true;

    IEnumerable<GetProductionOrder> _selectedProductionOrders = Array.Empty<GetProductionOrder>();

    bool _visible;


    private void OnSearch(OptionsSearchEventArgs<GetProductionOrder> e)
    {
        e.Items = ViewModel.GetProductionOrder.Where(i => i.DocNum.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.DocNum);
    }

    void UpdateGridSize(GridItemSize size)
    {
        if (size == GridItemSize.Xs)
        {
            _stringDisplay = "";
            _dataGrid = "width: 1600px;height:205px";
        }
        else
        {
            _stringDisplay = "Production Process";
            _dataGrid = "width: 1600px;height:405px";
        }
    }

    private void OnDeleteLine(int index)
    {
        ViewModel.ProductionProcessHeader.Data.RemoveAt(index);
    }

    private void OnAddLine()
    {
        ViewModel.ProductionProcessHeader.Data.Add(ViewModel.ProcessProductionLine);
    }

    private void OpenEdit(ProcessProductionLine productionProcess)
    {
        ViewModel.ProcessProductionLine = productionProcess;
        ViewModel.IsView = false;
    }

    async Task OnSaveTransaction()
    {
        await ErrorHandlingHelper.ExecuteWithHandlingAsync(async () =>
        {
            var result = await Validator!.ValidateAsync(ViewModel.ProductionProcessHeader).ConfigureAwait(false);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ToastService!.ShowError(error.ErrorMessage);
                }

                return;
            }

            _visible = true;
            await ViewModel.SubmitCommand.ExecuteAsync(null).ConfigureAwait(false);

            if (ViewModel.PostResponses.ErrorCode == "")
            {
                ViewModel.ProductionProcessHeader = new ProductionProcessHeader();
                ToastService.ShowSuccess("Success");
            }
            else
                ToastService.ShowError(ViewModel.PostResponses.ErrorMsg);
        }, ViewModel.PostResponses, ToastService).ConfigureAwait(false);
        _visible = false;
    }
}