using Microsoft.Extensions.Hosting;

namespace Tri_Wall.Shared.Services;

public class LoadMasterDataService : BackgroundService
{
    private readonly ILoadMasterData loadMasterData;
    public LoadMasterDataService(ILoadMasterData loadMasterData)
    {
        this.loadMasterData = loadMasterData;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //loadMasterData.LoadVendorMaster();
        await loadMasterData.LoadItemMaster();
    }
}
