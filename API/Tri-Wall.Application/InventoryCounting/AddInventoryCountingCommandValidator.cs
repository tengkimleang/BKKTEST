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
        }
    }
}
