using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.View;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.Client_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.Order_Service;
using Praca_Inżynierska_v1.Services.Product_service;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    internal class NewOrderViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();

        public ICommand SelectClientCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveOrderCommand { get; }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        private OrderItem _selectedOrderItem;
        public OrderItem SelectedOrderItem
        {
            get => _selectedOrderItem;
            set
            {
                _selectedOrderItem = value;
                OnPropertyChanged(nameof(SelectedOrderItem));
            }
        }

        public NewOrderViewModel(IClientService clientService, IProductService productService, IOrderService orderService)
        {
            _clientService = clientService;
            _productService = productService;
            _orderService = orderService;

            SelectClientCommand = new RelayCommand(_ => SelectClient());
            AddItemCommand = new RelayCommand(_ => AddNewItem());
            RemoveItemCommand = new RelayCommand(_ => RemoveSelectedItem(), _ => SelectedOrderItem != null);
            SaveOrderCommand = new RelayCommand(async _ => await SaveOrderAsync());

            LoadProducts();
        }

        private async void LoadProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            Products.Clear();
            foreach (var product in products)
                Products.Add(product);
        }

        private void SelectClient()
        {
            var window = new SelectClientWindow(_clientService);
            if (window.ShowDialog() == true && window.SelectedClient != null)
            {
                SelectedClient = window.SelectedClient;
            }
        }

        private void AddNewItem()
        {
            var selectWindow = new SelectProductWindow(_productService);
            if (selectWindow.ShowDialog() == true && selectWindow.SelectedProduct != null)
            {
                var selected = selectWindow.SelectedProduct;

                if (OrderItems.Any(o => o.ProductId == selected.ProductId))
                {
                    MyMessageBoxService.Show("Błąd", "Ten produkt został już dodany.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                    return;
                }

                var item = new OrderItem
                {
                    ProductId = selected.ProductId,
                    ProductName = selected.Name,
                    Unit = selected.Unit,
                    StockQuantity = selected.Quantity,
                    UnitPrice = selected.Price,
                    Quantity = 0,
                    Discount = selected.Discount,
                    DiscountStartDate = selected.DiscountStartDate,
                    DiscountEndDate = selected.DiscountEndDate
                };

                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(item.Quantity))
                    {
                        item.TotalPrice = Math.Round(item.Quantity * item.UnitPrice, 2);
                    }
                };

                OrderItems.Add(item);
            }
        }

        private void RemoveSelectedItem()
        {
            if (SelectedOrderItem != null)
            {
                OrderItems.Remove(SelectedOrderItem);
                SelectedOrderItem = null;
            }
        }

        private async Task SaveOrderAsync()
        {
            if (SelectedClient == null || OrderItems.Count == 0)
            {
                MyMessageBoxService.Show("Błąd", "Uzupełnij wszystkie wymagane pola!", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            if (OrderItems.Any(item => item.Quantity <= 0))
            {
                MyMessageBoxService.Show("Błąd", "Każda pozycja musi mieć ilość większą niż 0!", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            if (OrderItems.Any(item => item.Quantity == 0 || item.Quantity.ToString() == ""))
            {
                MyMessageBoxService.Show("Błąd", "Wszystkie pozycje muszą mieć uzupełnioną ilość!", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            var newOrder = new Order
            {
                ClientId = SelectedClient.ClientId,
                UserId = Session.CurrentUser?.UserId ?? throw new InvalidOperationException("Brak zalogowanego użytkownika."),
                OrderDate = DateTime.Now,
                Status = "Oczekujace",
                Notes = "",
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}"
            };

            try
            {
                await _orderService.AddOrderAsync(newOrder, OrderItems.ToList());
                foreach (var item in OrderItems)
                {
                    await _productService.DecreaseQuantityAsync(item.ProductId, item.Quantity);
                }

                MyMessageBoxService.Show("Sukces", "Zamówienie zapisane.", MyMessageBoxType.Success, MyMessageBoxButtons.OK);

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is NewOrderWindow)?
                    .Close();
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", $"Błąd zapisu: {ex.Message}", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }
    }
}