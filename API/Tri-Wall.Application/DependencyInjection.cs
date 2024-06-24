using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tri_Wall.Application.Authorize;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Application.GoodReceiptPo;
using Tri_Wall.Application.GoodReturn;
using Tri_Wall.Application.IssueForProductions;
using Tri_Wall.Application.Layout;
using Tri_Wall.Application.SaleOrder;

namespace Tri_Wall.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(option => option.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));
        services.AddScoped<IValidator<AuthorizeCommand>, AuthorizeCommandValidator>();
        services.AddScoped<IValidator<AddDeliveryOrderCommand>, AddDeliveryOrderCommandValidator>();
        services.AddScoped<IValidator<AddGoodReceiptPoCommand>, AddGoodReceiptPoCommandValidator>();
        services.AddScoped<IValidator<AddGoodReturnCommand>, AddGoodReturnCommandValidator>();
        services.AddScoped<IValidator<LayoutCommand>, LayoutCommandValidator>();
        services.AddScoped<IValidator<AddSaleOrderCommand>, AddSaleOrderCommandValidator>();
        services.AddScoped<IValidator<AddIssueForProductionCommand>, AddIssueForProductionCommandValidator>();
        return services;
    }
}
