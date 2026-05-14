using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services.Product_Service;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class DeliveryOrderViewModel : INotifyPropertyChanged
    {
        private readonly ProductServiceSql _productService;

        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<DeliveryItem> ItemsList { get; set; } = new();

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        private string _quantity;
        public string Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public ICommand AddItemCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public DeliveryOrderViewModel()
        {
            _productService = new ProductServiceSql(DBConnectionString.GetConnectionString());

            AddItemCommand = new RelayCommand(_ => AddItem());
            ConfirmCommand = new RelayCommand(_ => SaveDelivery());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());

            LoadProductsAsync();
        }

        private async void LoadProductsAsync()
        {
            try
            {
                var result = await _productService.GetAllProductsAsync();
                Products.Clear();
                foreach (var product in result)
                    Products.Add(product);
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show(
                    "Błąd",
                    "Błąd ładowania produktów: " + ex.Message,
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private void AddItem()
        {
            if (SelectedProduct == null)
            {
                MyMessageBoxService.Show(
                    "Brak produktu",
                    "Wybierz produkt, który chcesz dodać.",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
                return;
            }

            if (ItemsList.Any(x => x.ProductID == SelectedProduct.ProductId))
            {
                MyMessageBoxService.Show(
                    "Duplikat",
                    "Ten produkt znajduje się już na liście.",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
                return;
            }

            ItemsList.Add(new DeliveryItem
            {
                ProductID = SelectedProduct.ProductId,
                Name = SelectedProduct.Name
            });

            Quantity = "";
            SelectedProduct = null;
        }

        private void SaveDelivery()
        {
            if (ItemsList.Count == 0)
            {
                MyMessageBoxService.Show("Błąd", "Lista pozycji jest pusta.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            if (ItemsList.Any(item => item.Quantity <= 0))
            {
                MyMessageBoxService.Show("Błąd", "Wszystkie pozycje muszą mieć ilość większą niż 0.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            var result = MyMessageBoxService.Show(
                "Potwierdzenie",
                "Czy na pewno chcesz zapisać to zamówienie dostawy?",
                MyMessageBoxType.Question,
                MyMessageBoxButtons.YesNo
            );

            if (result != MyMessageBoxResult.Yes)
                return;

            try
            {
                using var conn = new Microsoft.Data.SqlClient.SqlConnection(DBConnectionString.GetConnectionString());
                conn.Open();
                var transaction = conn.BeginTransaction();

                var cmdDelivery = new Microsoft.Data.SqlClient.SqlCommand(@"
            INSERT INTO Dostawy (ID_Uzytkownika, Data_zamowienia, Status)
            OUTPUT INSERTED.ID_Dostawy
            VALUES (@uid, GETDATE(), 'Oczekująca')", conn, transaction);

                cmdDelivery.Parameters.AddWithValue("@uid", Session.CurrentUser.UserId);
                int deliveryId = (int)cmdDelivery.ExecuteScalar();

                foreach (var item in ItemsList)
                {
                    var cmdItem = new Microsoft.Data.SqlClient.SqlCommand(@"
                INSERT INTO Pozycja_dostawy (ID_Dostawy, ID_Produktu, Ilosc)
                VALUES (@did, @pid, @qty)", conn, transaction);

                    cmdItem.Parameters.AddWithValue("@did", deliveryId);
                    cmdItem.Parameters.AddWithValue("@pid", item.ProductID);
                    cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
                    cmdItem.Parameters.AddWithValue("@qtyReceived", DBNull.Value); // przy przyjęciu

                    cmdItem.ExecuteNonQuery();
                }

                transaction.Commit();
                MyMessageBoxService.Show("Sukces", "Zamówienie dostawy zostało zapisane.", MyMessageBoxType.Info, MyMessageBoxButtons.OK);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", "Błąd zapisu do bazy: " + ex.Message, MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}