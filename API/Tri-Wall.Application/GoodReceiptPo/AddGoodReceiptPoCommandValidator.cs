using FluentValidation;

namespace Tri_Wall.Application.GoodReceiptPo
{
    public class AddGoodReceiptPoCommandValidator : AbstractValidator<AddGoodReceiptPoCommand>
    {
        public AddGoodReceiptPoCommandValidator()
        {
            RuleFor(x => x.VendorCode).NotEmpty().WithMessage("VendorCode is required");
            RuleFor(x => x.Series).Equal(0).WithMessage("Series is required");
            RuleFor(x => x.DocDate).NotNull().WithMessage("DocDate is required");
            RuleFor(x => x.TaxDate).NotNull().WithMessage("TaxDate   is required");
            RuleFor(x => x.Lines).NotNull().WithMessage("Item Line is required")
                .ForEach(rule => rule.ChildRules(item =>
                {
                    item.RuleFor(i => i.ItemCode).NotEmpty().WithMessage("ItemCode must not be empty");
                    item.RuleFor(i => i.Qty).LessThanOrEqualTo(0).WithMessage("Qty should bigger than 0");
                    item.RuleFor(i => i.Price).LessThanOrEqualTo(0).WithMessage("Price should bigger than 0");
                }));

        }
    }
}
