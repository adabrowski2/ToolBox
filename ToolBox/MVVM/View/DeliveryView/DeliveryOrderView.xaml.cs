using Praca_Inżynierska_v1.MVVM.ViewModel;
using System.Windows;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class DeliveryOrderView : Window
    {
        public DeliveryOrderView()
        {
            InitializeComponent();

            var viewModel = new DeliveryOrderViewModel();
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }
    }
}