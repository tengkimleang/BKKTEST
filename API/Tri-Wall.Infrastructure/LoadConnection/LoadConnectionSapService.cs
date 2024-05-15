using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Application.Common.Interfaces.Setting;

namespace Tri_Wall.Infrastructure.LoadConnection;

public class LoadConnectionSapService : BackgroundService
{
    private readonly IConnection connection;
    public LoadConnectionSapService(IConnection connection)
    {
        this.connection = connection;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}  
