using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Application.InventoryCounting
{
    public class AddInventoryCountingCommandValidator : AbstractValidator<AddInventoryCountingCommand>
    {
        public AddInventoryCountingCommandValidator()
        {
            RuleFor(x => x.DocEntry).NotEmpty().WithMessage("DocEntry is not Empty");
            RuleFor(x => x.Series).NotEqual(0).WithMessage("Series is not Empty");
            RuleFor(x => x.Counters).NotNull().WithMessage("DocEntry is not Empty")
                .ForEach(rule => rule.ChildRules(item =>
                {
                    item.RuleFor(i => i.CountId).NotEqual(0).WithMessage("CountId must not be empty");
                }));
        }
    }
}
