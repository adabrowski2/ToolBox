using System.Windows;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class ClientTypeSelection : Window
    {
        public string SelectedType { get; private set; }

        public ClientTypeSelection()
        {
            InitializeComponent();
        }

        private void PersonButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = "Osoba fizyczna";
            DialogResult = true;
            Close();
        }

        private void CompanyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = "Jednoosobowa dzialalnosc gospodarcza";
            DialogResult = true;
            Close();
        }
    }
}
