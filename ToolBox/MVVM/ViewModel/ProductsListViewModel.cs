using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.Product_service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    internal class ProductsListViewModel : ViewModelBase
    {
        private readonly IProductService _productService;

        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<Product> FilteredProducts { get; set; } = new();

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand SaveQuantityChangesCommand { get; }

        public ProductsListViewModel(IProductService productService)
        {
            _productService = productService;

            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            SaveChangesCommand = new RelayCommand(_ => SaveChangesToDatabase());
            SaveQuantityChangesCommand = new RelayCommand(async _ => await SaveQuantityChangesAsync());

            LoadProducts();
        }

        internal async void LoadProducts()
        {
            try
            {
                var productList = await _productService.GetAllProductsAsync();

                Products.Clear();
                foreach (var p in productList)
                    Products.Add(p);

                FilteredProducts.Clear();
                foreach (var p in productList)
                    FilteredProducts.Add(p);
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show(
                    "Błąd ładowania",
                    $"Nie udało się wczytać produktów:\n{ex.Message}",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private void ExecuteSearch()
        {
            FilteredProducts.Clear();

            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                foreach (var p in Products)
                    FilteredProducts.Add(p);
            }
            else
            {
                var keyword = SearchKeyword.ToLower();
                foreach (var p in Products)
                {
                    if (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(keyword))
                        FilteredProducts.Add(p);
                }
            }
        }

        private async void SaveChangesToDatabase()
        {
            try
            {
                await _productService.UpdateProductsAsync(Products);

                MyMessageBoxService.Show(
                    "",
                    "Zapisano zmiany w bazie.",
                    MyMessageBoxType.Info,
                    MyMessageBoxButtons.OK
                );
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show(
                    "Błąd zapisu",
                    $"Wystąpił problem podczas zapisywania zmian:\n{ex.Message}",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private async Task SaveQuantityChangesAsync()
        {
            try
            {
                foreach (var product in FilteredProducts)
                {
                    await _productService.UpdateProductQuantityAsync(product.ProductId, product.Quantity);
                }

                MyMessageBoxService.Show("Sukces", "Zmiany zostały zapisane.", MyMessageBoxType.Success, MyMessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", $"Wystąpił problem: {ex.Message}", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }

    }
}