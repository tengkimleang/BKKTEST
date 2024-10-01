﻿using FluentValidation;
using Tri_Wall.Shared.Models.ReturnComponentProduction;

namespace Tri_Wall.Shared.Models.ReturnComponentProduction;

public class SerialReturnComponentProductionValidator : AbstractValidator<SerialReturnComponentProduction>
{
    public SerialReturnComponentProductionValidator()
    {
        RuleFor(x => x.Qty).NotEmpty().WithMessage("Item Code is Require");
        RuleFor(x => x.SerialCode).NotEmpty().WithMessage("Batch Code is Require");
        RuleFor(x => x).Custom((x, context) =>
        {
            if (context.RootContextData.TryGetValue("ExistingSerialNumbers", out var existingSerialNumbersObj) &&
                existingSerialNumbersObj is HashSet<string> existingSerialNumbers &&
                existingSerialNumbers.Contains(x.SerialCode))
            {
                context.AddFailure("SerialCode", "Duplicate serial number found");
            }
        });
    }
}