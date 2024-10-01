﻿using FluentValidation;

namespace Tri_Wall.Shared.Models.InventoryCounting;

public class InventoryCountingBatchValidator : AbstractValidator<InventoryCountingBatch>
{
    public InventoryCountingBatchValidator()
    {
        RuleFor(x => x.Qty).NotEmpty().WithMessage("Item Code is Require");
        RuleFor(x => x.BatchCode).NotEmpty().WithMessage("Batch Code is Require");
        RuleFor(x=>x).Custom((x,context)=>
        {
            if (x.QtyAvailable < x.Qty)
            {
                context.AddFailure("Qty","Qty is not available");
            }
        });
        
    }
}