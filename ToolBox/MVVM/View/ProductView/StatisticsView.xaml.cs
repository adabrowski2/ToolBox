using System.Windows;
using Praca_Inżynierska_v1.MVVM.ViewModel;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class StatisticsView : Window
    {
        public StatisticsView()
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel();


        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
