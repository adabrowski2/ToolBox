using System.Windows;
using Praca_Inżynierska_v1.MVVM.ViewModel;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class DeliveryListView : Window
    {
        public DeliveryListView()
        {
            InitializeComponent();
            DataContext = new DeliveryListViewModel();
        }

        private void Back_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}