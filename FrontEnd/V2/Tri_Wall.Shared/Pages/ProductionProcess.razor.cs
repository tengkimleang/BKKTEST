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

    [Inject] public IValidator<ProcessProductionLine>? ValidatorLine { get; init; }

    //ProductionProcessHeaderValidator
    private string _stringDisplay = "Production Process";
    string? _dataGrid = "width: 1600px;height:405px";
    private string _buttonAddName = "Add Line";
    protected void OnCloseOverlay() => _visible = true;

    IEnumerable<GetProductionOrder> _selectedProductionOrders = Array.Empty<GetProductionOrder>();
    IEnumerable<string> _selectProcessType = Array.Empty<string>();

    bool _visible;


    private void OnSearch(OptionsSearchEventArgs<GetProductionOrder> e)
    {
        e.Items = ViewModel.GetProductionOrder.Where(i => i.DocNum.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.DocNum);
    }

    private void OnSearchProductionNo(OptionsSearchEventArgs<string> e)
    {
        e.Items = ViewModel.ProcessType.Where(i => i.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i);
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

    private async Task OnAddLine()
    {
        var result = await ValidatorLine!.ValidateAsync(ViewModel.ProcessProductionLine).ConfigureAwait(false);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ToastService!.ShowError(error.ErrorMessage);
            }

            return;
        }

        if (ViewModel.ProcessProductionLine.Index == 0)
        {
            ViewModel.ProcessProductionLine.Index =
                ViewModel.ProductionProcessHeader.Data.MaxBy(x => x.Index)?.Index + 1 ?? 1;
            ViewModel.ProductionProcessHeader.Data.Add(ViewModel.ProcessProductionLine);
            ViewModel.ProcessProductionLine = new();
            _selectedProductionOrders = Array.Empty<GetProductionOrder>();
            _selectProcessType = Array.Empty<string>();
        }
        else
        {
            ViewModel.ProductionProcessHeader.Data
                .Where(i => i.Index == ViewModel.ProcessProductionLine.Index)
                .ToList()
                .ForEach(i =>
                {
                    i.DocNum = ViewModel.ProcessProductionLine.DocNum;
                    i.ProcessStage = ViewModel.ProcessProductionLine.ProcessStage;
                    i.ProductionNo = ViewModel.ProcessProductionLine.ProductionNo;
                    i.Status = ViewModel.ProcessProductionLine.Status;
                });
            ViewModel.ProcessProductionLine = new();
            _selectedProductionOrders = Array.Empty<GetProductionOrder>();
            _selectProcessType = Array.Empty<string>();
            _buttonAddName="Add Line";
        }
    }

    private void OpenEdit(ProcessProductionLine productionProcess)
    {
        ViewModel.ProcessProductionLine = new()
        {
            Index = productionProcess.Index,
            DocNum = productionProcess.DocNum,
            ProcessStage = productionProcess.ProcessStage,
            ProductionNo = productionProcess.ProductionNo,
            Status = productionProcess.Status,
        };
        _selectedProductionOrders = ViewModel.GetProductionOrder.Where(i => i.DocNum == productionProcess.DocNum);
        _selectProcessType = ViewModel.ProcessType.Where(i => i == productionProcess.ProcessStage);
        _buttonAddName="Update Line";
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