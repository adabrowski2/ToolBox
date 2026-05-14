using System.Collections.ObjectModel;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.View;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.Delivery_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class DeliveryListViewModel : ViewModelBase
    {
        private readonly DeliveryServiceSQL _deliveryService;

        public ObservableCollection<Delivery> Deliveries { get; set; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand ShowDetailsCommand { get; }

        private Delivery _selectedDelivery;
        public Delivery SelectedDelivery
        {
            get => _selectedDelivery;
            set
            {
                _selectedDelivery = value;
                OnPropertyChanged();
            }
        }

        public DeliveryListViewModel()
        {
            _deliveryService = new DeliveryServiceSQL(DBConnectionString.GetConnectionString());

            RefreshCommand = new RelayCommand(_ => LoadDeliveries());
            ShowDetailsCommand = new RelayCommand(_ => ShowDeliveryDetails());

            LoadDeliveries();
        }

        private void LoadDeliveries()
        {
            var data = _deliveryService.GetAllDeliveries();
            Deliveries.Clear();
            foreach (var d in data)
                Deliveries.Add(d);
        }

        private void ShowDeliveryDetails()
        {
            if (SelectedDelivery == null)
            {
                MyMessageBoxService.Show("Uwaga", "Najpierw wybierz dostawę.", MyMessageBoxType.Info, MyMessageBoxButtons.OK);
                return;
            }

            var detailsWindow = new DeliveryDetailsView(SelectedDelivery);
            detailsWindow.ShowDialog();
            LoadDeliveries();
        }

    }
}