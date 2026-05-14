using System.Windows;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services.Product_service;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class SelectProductWindow : Window
    {
        public Product SelectedProduct { get; private set; }

        internal SelectProductWindow(IProductService productService)
        {
            InitializeComponent();
            var vm = new SelectProductViewModel(productService);
            vm.ProductSelected += OnProductSelected;
            DataContext = vm;
        }

        private void OnProductSelected(Product product)
        {
            SelectedProduct = product;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}