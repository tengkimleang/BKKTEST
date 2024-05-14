using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Shared
{
    public class LoadMasterData : ILoadMasterData
    {
        readonly ApiService apiService;
        public ObservableCollection<Items> GetItems = new();
        public ObservableCollection<Vendors> GetVendors = new();
        public LoadMasterData(ApiService apiService)
        {
            this.apiService = apiService;
        }
        public async Task LoadItemMaster()
        {
            var result = await apiService.GetItems();
            if (result.ErrorCode == "")
            {
                GetItems = new ObservableCollection<Items>(result.Data ?? new());
            }
        }

        public async Task LoadVendorMaster()
        {
            var result = await apiService.GetVendors();
            if (result.ErrorCode == "")
            {
                GetVendors = result.Data?? new();
            }
        }
    }
}
