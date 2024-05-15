using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.Common;
using Tri_Wall.Infrastructure.Common.Setting;
using Tri_Wall.Infrastructure.Common.Persistence;
using Tri_Wall.Infrastructure.Common.QueryData;
using Tri_Wall.Infrastructure.LoadConnection;

namespace Tri_Wall.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configure)
    {
        // Add connection settings to the service collection
        services.Configure<ConnectionSettings>(configure.GetSection(ConnectionSettings.SectionName));
        services.Configure<JwtSettings>(configure.GetSection(JwtSettings.SectionName));
        // Add connection provider to the service collection
        services.AddSingleton<IConnection, Connection>();
        // Add Unit of Work to the service collection
        services.AddScoped<IUnitOfWork, Connection>();
        // Add Query Data to the service collection
        services.AddControllers().AddNewtonsoftJson();
        //// Add JWT token generator to the service collection
        services.AddSingleton<IJwtRegister, JwtRegister>();
        services.AddSingleton<IConvertRecordsetToDataTable, ConvertRecordsetToDataTable>();
        // Add data provider to the service collection
        services.AddSingleton<IDataProviderRepository, DataProviderRepository>();
        services.AddSingleton<IReportLayout, ReportLayout>();
        // Add user repository to the service collection
        services.AddHostedService<LoadConnectionSapService>();
        return services;
    }
}
