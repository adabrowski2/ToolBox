using System.Text.RegularExpressions;
using System.Windows;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class ClientDetails : Window
    {

        private readonly bool _isReadOnly;

        public ClientDetails(Client client, bool isReadOnly = false)
        {
            InitializeComponent();
            DataContext = client;
            _isReadOnly = isReadOnly;

            if (_isReadOnly)
                SetReadOnlyMode();
        }

        private bool ValidateClientData(Client client)
        {
            // 1. Sprawdzenie wymaganych pól
            if (string.IsNullOrWhiteSpace(client.FirstName) ||
                string.IsNullOrWhiteSpace(client.LastName) ||
                string.IsNullOrWhiteSpace(client.Email) ||
                string.IsNullOrWhiteSpace(client.Phone) ||
                string.IsNullOrWhiteSpace(client.Address))
            {
                MyMessageBoxService.Show("Błąd", "Wszystkie pola muszą być uzupełnione.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            // 2. Imię i nazwisko – tylko litery i myślniki, min. 2 znaki
            if (!Regex.IsMatch(client.FirstName, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-]{2,}$"))
            {
                MyMessageBoxService.Show("Błąd", "Imię może zawierać tylko litery (min. 2 znaki).", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            if (!Regex.IsMatch(client.LastName, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-]{2,}$"))
            {
                MyMessageBoxService.Show("Błąd", "Nazwisko może zawierać tylko litery (min. 2 znaki).", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            // 3. E-mail
            if (!Regex.IsMatch(client.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MyMessageBoxService.Show("Błąd", "Podaj poprawny adres e-mail.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            // 4. Telefon – dokładnie w formacie 123-456-789
            if (!Regex.IsMatch(client.Phone, @"^\d{3}-\d{3}-\d{3}$"))
            {
                MyMessageBoxService.Show("Błąd", "Telefon musi być w formacie 123-456-789.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            // 5. Adres – musi zawierać kod pocztowy w formacie 00-000
            if (!Regex.IsMatch(client.Address, @"\b\d{2}-\d{3}\b"))
            {
                MyMessageBoxService.Show("Błąd", "Adres musi zawierać kod pocztowy w formacie 00-000.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return false;
            }

            // 6. Walidacja JDG (NIP i REGON wymagane tylko dla firm)
            if (client.ClientType == "Jednoosobowa dzialalnosc gospodarcza")
            {
                if (string.IsNullOrWhiteSpace(client.CompanyName))
                {
                    MyMessageBoxService.Show("Błąd", "Nazwa firmy nie może być pusta.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(client.NIP) || !Regex.IsMatch(client.NIP, @"^\d{10}$"))
                {
                    MyMessageBoxService.Show("Błąd", "NIP musi zawierać dokładnie 10 cyfr.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(client.REGON) && !Regex.IsMatch(client.REGON, @"^\d{9}$"))
                {
                    MyMessageBoxService.Show("Błąd", "REGON (jeśli podany) musi zawierać dokładnie 9 cyfr.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                    return false;
                }
            }

            // Wszystko OK
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Client client)
            {
                if (!ValidateClientData(client))
                    return;

                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetReadOnlyMode()
        {
            FirstNameTextBox.IsReadOnly = true;
            LastNameTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            PhoneTextBox.IsReadOnly = true;
            AddressTextBox.IsReadOnly = true;
            CompanyNameTextBox.IsReadOnly = true;
            NIPTextBox.IsReadOnly = true;
            REGONTextBox.IsReadOnly = true;

            SaveButton.Visibility = Visibility.Collapsed;
        }
    }
}
