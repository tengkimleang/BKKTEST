using FluentValidation;
using Tri_Wall.Shared.Models.ReceiptFinishGood;

namespace Tri_Wall.Shared.Models.ReceiptFinishGood;

public class ReceiptFinishGoodBatchValidator : AbstractValidator<ReceiptFinishGoodBatch>
{
    public ReceiptFinishGoodBatchValidator()
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