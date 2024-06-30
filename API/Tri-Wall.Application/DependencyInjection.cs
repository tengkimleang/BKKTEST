using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
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
        //services.AddValidatorsFromAssemblyContaining<AuthorizeCommand>();
        //services.AddValidatorsFromAssemblyContaining<AddDeliveryOrderCommand>();
        //services.AddValidatorsFromAssemblyContaining<AddGoodReceiptPoCommand>();
        //services.AddValidatorsFromAssemblyContaining<AddGoodReturnCommand>();
        //services.AddValidatorsFromAssemblyContaining<LayoutCommand>();
        //services.AddValidatorsFromAssemblyContaining<AddSaleOrderCommand>();
        //services.AddValidatorsFromAssemblyContaining<AddIssueForProductionCommand>();
        // Manually register all validators found in the assembly
        var assembly = Assembly.GetAssembly(typeof(DependencyInjection)); // Replace DependencyInjection with any class within your target assembly
        var validatorTypes = assembly?.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        foreach (var validatorType in validatorTypes)
        {
            var interfaceType = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddScoped(interfaceType, validatorType);
        }

        return services;
    }
}
