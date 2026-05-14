using System.Collections.ObjectModel;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services.Client_Service;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class SelectClientViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;

        public ObservableCollection<Client> AllClients { get; set; } = new();
        public ObservableCollection<Client> FilteredClients { get; set; } = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilter();
            }
        }

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

        public ICommand ConfirmCommand { get; }

        public event Action<Client> ClientSelected;
        public event Action Cancelled;

        public SelectClientViewModel(IClientService clientService)
        {
            _clientService = clientService;
            ConfirmCommand = new RelayCommand(_ => Confirm());
            LoadClients();
        }

        private async void LoadClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            AllClients.Clear();
            FilteredClients.Clear();

            foreach (var client in clients)
            {
                AllClients.Add(client);
                FilteredClients.Add(client);
            }
        }

        private void ApplyFilter()
        {
            FilteredClients.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var client in AllClients)
                    FilteredClients.Add(client);

                return;
            }

            string lowered = SearchText.ToLower();

            var filtered = AllClients.Where(c =>
                (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLower().Contains(lowered)) ||
                (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.ToLower().Contains(lowered)) ||
                (!string.IsNullOrEmpty(c.Address) && c.Address.ToLower().Contains(lowered)) ||
                (!string.IsNullOrEmpty(c.Phone) && c.Phone.ToLower().Contains(lowered)));

            foreach (var client in filtered)
                FilteredClients.Add(client);
        }

        private void Confirm()
        {
            if (SelectedClient != null)
            {
                ClientSelected?.Invoke(SelectedClient);
            }
        }
    }
}
