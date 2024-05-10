﻿using FluentValidation;
using Tri_Wall.Application.DeliveryOrder;

namespace Tri_Wall.Application;

public class AddDeliveryOrderCommandValidator : AbstractValidator<AddDeliveryOrderCommand>
{
    public AddDeliveryOrderCommandValidator()
    {
        RuleFor(x => x.CardCode).NotEmpty().WithMessage("CardCode Need to Specify");
        RuleFor(x => x.Series).NotEqual(0).WithMessage("Series need to Specify");
        RuleFor(x => x.Lines).NotNull().WithMessage("Item Line Not Null")
            .ForEach(rule => rule.ChildRules(item =>
                {
                    item.RuleFor(i => i.ItemCode).NotEmpty().WithMessage("ItemCode must not be empty");
                    item.RuleFor(i => i.Qty).LessThanOrEqualTo(0).WithMessage("Qty should bigger than 0");
                    item.RuleFor(i => i.Price).LessThanOrEqualTo(0).WithMessage("Price should bigger than 0");
                }));
    }
}