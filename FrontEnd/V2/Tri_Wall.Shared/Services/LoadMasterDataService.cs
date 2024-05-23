﻿using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;

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
        await loadMasterData.LoadItemMaster();
        await loadMasterData.LoadVendorMaster();
        await loadMasterData.LoadContactPersonMaster();
        await loadMasterData.LoadGetTaxPurchaseMaster();
        await loadMasterData.LoadGetWarehouseMaster();
    }
}