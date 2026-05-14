using System.Collections.ObjectModel;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.View;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.Client_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class ClientsListViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;

        public ICommand AddClientCommand { get; }
        public ICommand SearchCommand { get; }

/*        public ICommand SaveChangesCommand { get; }*/
/*        public ICommand EditClientCommand { get; }*/
        public ICommand ShowDetailsCommand { get; }

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Client> FilteredClients { get; set; }

        public bool CanAddClient =>
            Session.CurrentUser?.Role == "doradca_handlowy" ||
            Session.CurrentUser?.Role == "kierownik_dzialu_handlowego";

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

        public ClientsListViewModel(IClientService clientService)
        {
            _clientService = clientService;

            Clients = new ObservableCollection<Client>();
            FilteredClients = new ObservableCollection<Client>();

            LoadClients();

            SearchCommand = new RelayCommand(_ => Search());
/*          EditClientCommand = new RelayCommand(_ => EditClient(), _ => SelectedClient != null);*/
            AddClientCommand = new RelayCommand(_ => AddClient());
            ShowDetailsCommand = new RelayCommand(_ => ShowClientDetails(), _ => SelectedClient != null);
        }

        private async void LoadClients()
        {
            try
            {
                var list = await _clientService.GetAllClientsAsync();
                Clients.Clear();
                FilteredClients.Clear();

                foreach (var c in list)
                {
                    Clients.Add(c);
                    FilteredClients.Add(c);
                }
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", "Błąd podczas wczytywania klientów:\n" + ex.Message, MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }

        private void Search()
        {
            FilteredClients.Clear();

            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                foreach (var c in Clients)
                    FilteredClients.Add(c);
            }
            else
            {
                var keyword = SearchKeyword.ToLower();

                foreach (var c in Clients)
                {
                    if ((c.FirstName != null && c.FirstName.ToLower().Contains(keyword)) ||
                        (c.LastName != null && c.LastName.ToLower().Contains(keyword)) ||
                        (c.CompanyName != null && c.CompanyName.ToLower().Contains(keyword)) ||
                        (c.Email != null && c.Email.ToLower().Contains(keyword)))
                    {
                        FilteredClients.Add(c);
                    }
                }
            }
        }



/*        private void EditClient()
        {
            if (SelectedClient == null)
                return;

            var clonedClient = SelectedClient.Clone();
            var editWindow = new ClientDetails(clonedClient);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                SelectedClient.CopyFrom(clonedClient);
            }
        }*/

        private void AddClient()
        {
            var typeWindow = new ClientTypeSelection();
            bool? result = typeWindow.ShowDialog();

            if (result == true && !string.IsNullOrWhiteSpace(typeWindow.SelectedType))
            {
                var newClient = new Client
                {
                    ClientType = typeWindow.SelectedType
                };

                var clientDetailsWindow = new ClientDetails(newClient);
                bool? saved = clientDetailsWindow.ShowDialog();

                if (saved == true)
                {
                    Clients.Add(newClient);
                    FilteredClients.Add(newClient);
                    SaveSingleClientAsync(newClient);
                }
            }
        }

        private void ShowClientDetails()
        {
            if (SelectedClient == null)
                return;

            var clonedClient = SelectedClient.Clone();
            var detailsWindow = new ClientDetails(clonedClient, isReadOnly: true);
            detailsWindow.ShowDialog();
        }

        private async void SaveSingleClientAsync(Client client)
        {
            try
            {
                await _clientService.AddClientAsync(client);
                MyMessageBoxService.Show("Sukces", "Dodano nowego klienta.", MyMessageBoxType.Success, MyMessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", $"Nie udało się zapisać klienta:\n{ex.Message}", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }
    }
}