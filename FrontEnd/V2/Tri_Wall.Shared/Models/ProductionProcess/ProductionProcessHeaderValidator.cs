using FluentValidation;

namespace Tri_Wall.Shared.Models.ProductionProcess;

public class ProductionProcessHeaderValidator : AbstractValidator<ProductionProcessHeader>
{
    public ProductionProcessHeaderValidator()
    {
        RuleFor(x => x.Data).NotEmpty().WithMessage("Lines is require")
            .ForEach(rule => rule.ChildRules(item =>
            {
                item.RuleFor(i => i.ProductionNo).NotEqual(0).WithMessage("ProductionNo must not be empty");
                item.RuleFor(i => i.ProcessStage).NotEmpty().WithMessage("ProcessStage must not be empty");
                item.RuleFor(i => i.Status).NotEmpty().WithMessage("Status must not be empty");
            }));
    }
}