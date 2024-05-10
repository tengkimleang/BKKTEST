using FluentValidation;

namespace Tri_Wall.Application.GoodReturn;

public class AddGoodReturnCommandValidator : AbstractValidator<AddGoodReturnCommand>
{
    public AddGoodReturnCommandValidator()
    {
        RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Vendor is Require");
        RuleFor(x => x.Series).Equal(0).WithMessage("Series is Require");
        RuleFor(x => x.DocDate).NotNull().WithMessage("DocDate is Require");
        RuleFor(x => x.TaxDate).NotNull().WithMessage("TaxDate is Require");
        RuleFor(x => x.Lines).NotNull().WithMessage("Lines is Require")
            .ForEach(rule => rule.ChildRules(item =>
            {
                item.RuleFor(i => i.ItemCode).NotEmpty().WithMessage("ItemCode must not be empty");
                item.RuleFor(i => i.Qty).LessThanOrEqualTo(0).WithMessage("Qty should bigger than 0");
                item.RuleFor(i => i.Price).LessThanOrEqualTo(0).WithMessage("Price should bigger than 0");
            }));
    }
}
