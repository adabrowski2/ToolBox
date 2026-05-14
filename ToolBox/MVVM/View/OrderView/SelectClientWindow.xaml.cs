using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services.Client_Service;
using System.Windows;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class SelectClientWindow : Window
    {
        public Client SelectedClient { get; private set; }

        public SelectClientWindow(IClientService clientService)
        {
            InitializeComponent();

            var vm = new SelectClientViewModel(clientService);
            vm.ClientSelected += OnClientSelected;
            vm.Cancelled += OnCancelled;
            DataContext = vm;
        }

        private void OnClientSelected(Client client)
        {
            SelectedClient = client;
            DialogResult = true;
            Close();
        }

        private void OnCancelled()
        {
            DialogResult = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}