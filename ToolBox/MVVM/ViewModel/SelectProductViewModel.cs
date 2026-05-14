using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    internal class SelectProductViewModel : ViewModelBase
    {
        private readonly Services.Product_service.IProductService _productService;

        public ObservableCollection<Product> FilteredProducts { get; set; } = new();
        private ObservableCollection<Product> _allProducts = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterProducts();
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public ICommand ConfirmCommand { get; }

        public event Action<Product> ProductSelected;

        internal SelectProductViewModel(Services.Product_service.IProductService productService)
        {
            _productService = productService;
            ConfirmCommand = new RelayCommand(_ => ConfirmSelection(), _ => SelectedProduct != null);
            LoadProducts();
        }

        private async void LoadProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            _allProducts = new ObservableCollection<Product>(products);
            FilterProducts();
        }

        private void FilterProducts()
        {
            FilteredProducts.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allProducts
                : new ObservableCollection<Product>(_allProducts.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Unit) && p.Unit.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    p.Price.ToString(CultureInfo.InvariantCulture).Contains(SearchText)));

            foreach (var product in filtered)
                FilteredProducts.Add(product);
        }

        private void ConfirmSelection()
        {
            if (SelectedProduct == null)
            {
                MyMessageBoxService.Show("Błąd", "Wybierz produkt", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }
            ProductSelected?.Invoke(SelectedProduct);
        }
    }
}