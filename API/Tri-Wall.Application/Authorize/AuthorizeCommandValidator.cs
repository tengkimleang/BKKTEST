using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Application.Authorize;

public class AuthorizeCommandValidator : AbstractValidator<AuthorizeCommand>
{
    public AuthorizeCommandValidator()
    {
        RuleFor(x => x.Account).NotEmpty().WithMessage("Account Need to Specify");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password need to Specify");
    }
}
