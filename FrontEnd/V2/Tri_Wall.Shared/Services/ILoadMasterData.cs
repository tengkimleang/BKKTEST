﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;

public interface ILoadMasterData
{
    public ObservableCollection<Items> GetItems { get; }
    public ObservableCollection<Vendors> GetVendors { get; }
    public ObservableCollection<ContactPersons> GetContactPersons { get; }
    public ObservableCollection<VatGroups> GetTaxPurchases { get; }
    public ObservableCollection<Warehouses> GetWarehouses { get; }
    Task LoadItemMaster();
    Task LoadVendorMaster();
    Task LoadContactPersonMaster();
    Task LoadGetTaxPurchaseMaster();
    Task LoadGetWarehouseMaster();
}