using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Shared
{
    public class LoadMasterData(ApiService apiService) : ILoadMasterData
    {
        readonly ApiService apiService = apiService;
        private ObservableCollection<Items> GetItems=new();
        private ObservableCollection<Vendors> GetVendors= new();
        private ObservableCollection<ContactPerson> GetContactPersons = new();
        

        ObservableCollection<Items> ILoadMasterData.GetItems => GetItems;

        ObservableCollection<Vendors> ILoadMasterData.GetVendors => GetVendors;

        ObservableCollection<ContactPerson> ILoadMasterData.GetContactPersons => GetContactPersons;

        public async Task LoadContactPersonMaster()
        {
            var result = await apiService.GetContactPersons();
            if (result.ErrorCode == "")
            {
                GetContactPersons = new ObservableCollection<ContactPerson>(result.Data ?? new());
            }
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
