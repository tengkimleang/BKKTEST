using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.Shared
{
    public class LoadMasterData(ApiService apiService) : ILoadMasterData
    {
        readonly ApiService apiService = apiService;
        private ObservableCollection<Items> _getItems=new();
        private ObservableCollection<Vendors> _getVendors= new();
        private ObservableCollection<ContactPersons> _getContactPersons = new();
        private ObservableCollection<VatGroups> _getTaxPurchases = new();
        private ObservableCollection<Warehouses> _getWarehouses = new();

        public ObservableCollection<VatGroups> GetTaxPurchases => _getTaxPurchases;

        public ObservableCollection<Warehouses> GetWarehouses => _getWarehouses;

        ObservableCollection<Items> ILoadMasterData.GetItems => _getItems;

        ObservableCollection<Vendors> ILoadMasterData.GetVendors => _getVendors;

        ObservableCollection<ContactPersons> ILoadMasterData.GetContactPersons => _getContactPersons;

        public async Task LoadContactPersonMaster()
        {
            var result = await apiService.GetContactPersons();
            if (result.ErrorCode == "")
            {
                _getContactPersons = new ObservableCollection<ContactPersons>(result.Data ?? new());
            }
        }

        public async Task LoadGetTaxPurchaseMaster()
        {
            var result = await apiService.GetContactPersons();
            if (result.ErrorCode == "")
            {
                _getContactPersons = new ObservableCollection<ContactPersons>(result.Data ?? new());
            }
        }

        public async Task LoadGetWarehouseMaster()
        {
            var result = await apiService.GetContactPersons();
            if (result.ErrorCode == "")
            {
                _getContactPersons = new ObservableCollection<ContactPersons>(result.Data ?? new());
            }
        }

        public async Task LoadItemMaster()
        {
            var result = await apiService.GetItems();
            if (result.ErrorCode == "")
            {
                _getItems = new ObservableCollection<Items>(result.Data ?? new());
            }
        }

        public async Task LoadVendorMaster()
        {
            var result = await apiService.GetVendors();
            if (result.ErrorCode == "")
            {
                _getVendors = result.Data?? new();
            }
        }
    }
}
