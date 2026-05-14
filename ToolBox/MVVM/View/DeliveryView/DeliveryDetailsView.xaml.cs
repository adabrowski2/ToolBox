using System.Windows;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.ViewModel;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class DeliveryDetailsView : Window
    {
        public DeliveryDetailsView(Delivery delivery)
        {
            InitializeComponent();
            var viewModel = new DeliveryDetailsViewModel(delivery);
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }
    }
}