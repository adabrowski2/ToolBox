using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services.Client_Service;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using System.Windows;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class ClientsList : Window
    {
        public ClientsList()
        {
            InitializeComponent();

            if (Session.CurrentUser?.Role == "magazynier" || Session.CurrentUser?.Role == "kierownik_magazynu")
            {
                EditClientPanel.Visibility = Visibility.Collapsed;
            }

            // 1. Pobieramy connection string z klasy pomocniczej
            var connectionString = DBConnectionString.GetConnectionString();

            // 2. Tworzymy serwis do obsługi klientów
            var clientService = new ClientServiceSql(connectionString);

            // 3. Inicjalizujemy ViewModel
            var viewModel = new ClientsListViewModel(clientService);

            // 4. Przypisujemy ViewModel jako DataContext
            DataContext = viewModel;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            WindowHelpers.DragWindow(this, e);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Close(this);
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Minimize(this);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.MaximizeOrRestore(this);
        }
    }
}
