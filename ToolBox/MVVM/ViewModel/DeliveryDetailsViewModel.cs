using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.Delivery_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class DeliveryDetailsViewModel : INotifyPropertyChanged
    {
        private readonly DeliveryServiceSQL _deliveryService;
        public ObservableCollection<DeliveryItem> Items { get; set; } = new();
        public Delivery Delivery { get; }

        public ICommand AcceptDeliveryCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public DeliveryDetailsViewModel(Delivery delivery)
        {
            Delivery = delivery;
            _deliveryService = new DeliveryServiceSQL(DBConnectionString.GetConnectionString());

            AcceptDeliveryCommand = new RelayCommand(_ => AcceptDelivery(), _ => Delivery.Status == "Oczekujaca");
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());

            LoadItems();
        }

        private void LoadItems()
        {
            var result = _deliveryService.GetDeliveryItems(Delivery.DeliveryId);
            Items.Clear();
            foreach (var item in result)
                Items.Add(item);
        }

        private void AcceptDelivery()
        {
            if (Items.Any(i => i.ReceivedQuantity < 0))
            {
                MyMessageBoxService.Show("Błąd", "Nie można przyjąć ujemnej ilości.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            var confirm = MyMessageBoxService.Show(
                "Potwierdzenie",
                "Czy na pewno chcesz przyjąć tę dostawę?\nWprowadzone ilości zostaną zapisane.",
                MyMessageBoxType.Question,
                MyMessageBoxButtons.YesNo
            );

            if (confirm != MyMessageBoxResult.Yes)
                return;

            try
            {
                _deliveryService.MarkDeliveryAsReceivedWithQuantities(Delivery.DeliveryId, Items.ToList());
                MyMessageBoxService.Show("Sukces", "Dostawa została oznaczona jako przyjęta.", MyMessageBoxType.Info, MyMessageBoxButtons.OK);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show("Błąd", "Nie udało się przyjąć dostawy: " + ex.Message, MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}