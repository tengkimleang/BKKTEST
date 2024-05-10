using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Application.Layout;

public class LayoutCommandValidator : AbstractValidator<LayoutCommand>
{
    public LayoutCommandValidator()
    {
        RuleFor(x => x.LayoutCode).NotEmpty().WithMessage("LayoutCode Need to Specify");
        RuleFor(x => x.DocEntry).NotEmpty().WithMessage("DocEntry need to Specify");
    }
}
