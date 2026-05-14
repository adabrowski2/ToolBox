using System.Windows;
using System.Windows.Input;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services.Client_Service;
using Praca_Inżynierska_v1.Services.Product_Service;
using Praca_Inżynierska_v1.Services.Order_Service;
using Praca_Inżynierska_v1.Services.DBConnectionString;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class NewOrderWindow : Window
    {
        public NewOrderWindow()
        {
            InitializeComponent();

            string connectionString = DBConnectionString.GetConnectionString();

            var clientService = new ClientServiceSql(connectionString);
            var productService = new ProductServiceSql(connectionString);
            var orderService = new OrderServiceSQL(connectionString);

            DataContext = new NewOrderViewModel(clientService, productService, orderService);
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            WindowHelpers.DragWindow(this, e);
        }
    }
}
