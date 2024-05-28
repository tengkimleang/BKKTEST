using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tri_Wall.Shared.Models;
using Tri_Wall.Shared.Models.GoodReceiptPo;
using Tri_Wall.Shared.Services;

namespace Tri_Wall.Shared.ViewModels;

public partial class GoodReceptPoViewModel(ApiService apiService, ILoadMasterData loadMasterData) : ViewModelBase
{
    [ObservableProperty]
    GoodReceiptPoHeader _goodReceiptPoForm=new();

    [ObservableProperty]
    ObservableCollection<Series> _series = new();

    [ObservableProperty]
    ObservableCollection<Vendors> _vendors = loadMasterData.GetVendors;

    [ObservableProperty]
    ObservableCollection<ContactPersons> _contactPeople = loadMasterData.GetContactPersons;

    [ObservableProperty]
    ObservableCollection<Items> _items = loadMasterData.GetItems;

    [ObservableProperty]
    ObservableCollection<VatGroups> _taxPurchases = loadMasterData.GetTaxPurchases;

    [ObservableProperty]
    ObservableCollection<Warehouses> _warehouses = loadMasterData.GetWarehouses;

    [ObservableProperty]
    PostResponse _postResponses = new();
    
    [ObservableProperty]
    ObservableCollection<TotalItemCount> _totalItemCount=new();
    
    [ObservableProperty]
    ObservableCollection<GetListData> _getListData = new();
    
    public override async Task Loaded()
    {
        Series = await CheckingValueT(Series, async () =>
                 (await apiService.GetSeries("20")).Data ?? new());
        Vendors = await CheckingValueT(Vendors, async () =>
                        (await apiService.GetVendors()).Data ?? new());
        ContactPeople = await CheckingValueT(ContactPeople, async () =>
                    (await apiService.GetContactPersons()).Data ?? new());
        Items = await CheckingValueT(Items, async () =>
                    (await apiService.GetItems()).Data ?? new());
        TaxPurchases = await CheckingValueT(TaxPurchases, async () =>
                    (await apiService.GetTaxPurchases()).Data ?? new());
        Warehouses = await CheckingValueT(Warehouses, async () =>
                    (await apiService.GetWarehouses()).Data ?? new());
        TotalItemCount = (await apiService.GetTotalItemCount("GoodReceiptPo")).Data ?? new();
    }
    [RelayCommand]
    async Task Submit()
    {
        GoodReceiptPoForm.ContactPersonCode = "0";
        PostResponses = await apiService.PostGoodReceptPo(GoodReceiptPoForm);
    }
    
    [RelayCommand]
    async Task OnGetGoodReceiptPo(string perPage)
    {
        try
        {
            GetListData = (await apiService.GetListGoodReceiptPo(perPage)).Data ?? new();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
