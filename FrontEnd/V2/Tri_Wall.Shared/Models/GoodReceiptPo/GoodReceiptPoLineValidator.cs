using FluentValidation;

namespace Tri_Wall.Shared.Models.GoodReceiptPo;

public class GoodReceiptPoLineValidator : AbstractValidator<GoodReceiptPoLine>
{
    public GoodReceiptPoLineValidator()
    {
        RuleFor(x => x.ItemCode).NotEmpty().WithMessage("Item Code is Require");
        RuleFor(x => x.Qty).NotNull().WithMessage("Qty is Require");
        RuleFor(x => x.Qty).GreaterThan(0).WithMessage("Qty is Require");
        RuleFor(x => x.Serials).Must((x, serials) => x.ManageItem == "S" ? serials?.Count > 0 : true)
            .WithMessage("Serial is Require")
            .Must((x, serials) => x.ManageItem == "S" ? serials?.Sum(b => b.Qty) == x.Qty : true)
            .WithMessage("Serial is not bigger than Qty")
            .ForEach(rule => rule.ChildRules(item =>
            {
                item.RuleFor(i => i.SerialCode).NotEmpty().WithMessage("SerialCode must not be empty");
                item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Qty should bigger than 0");
            }));
        RuleFor(x => x.Batches).Must((x, batches)=> x.ManageItem=="B"? batches?.Count > 0 : true)
            .WithMessage("Batch is Require")
            .Must((x, batches) => x.ManageItem == "B" ? batches?.Sum( b => b.Qty) == x.Qty : true)
            .WithMessage("Batch Qty must be equal to Qty")
            .ForEach(rule => rule.ChildRules(item =>
            {
                item.RuleFor(i => i.BatchCode).NotEmpty().WithMessage("Batch Number must not be empty");
                item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Qty should bigger than 0");
            }));
    }
}
