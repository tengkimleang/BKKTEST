﻿using FluentValidation;

namespace Tri_Wall.Shared.Models.ProductionProcess;

public class ProductionProcessHeaderValidator : AbstractValidator<ProductionProcessHeader>
{
    public ProductionProcessHeaderValidator()
    {
        RuleFor(x => x.Data).NotEmpty().WithMessage("Lines is require")
            .ForEach(rule => rule.SetValidator(new ProcessProductionLineValidator()));
    }
}