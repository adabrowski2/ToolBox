using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.User_Service;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            string connectionString = DBConnectionString.GetConnectionString();

            var userService = new UserServiceSql(connectionString);
            var vm = new LoginViewModel(userService);

            vm.LoginSucceeded += OnLoginSucceeded;
            vm.LoginFailed += OnLoginFailed;

            this.DataContext = vm;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnLoginSucceeded()
        {
            var mainW = new MainWindow();
            mainW.Show();
            this.Close();
        }

        private void OnLoginFailed(string errorMessage)
        {
            MyMessageBoxService.Show(
                "Błąd logowania",
                errorMessage,
                MyMessageBoxType.Error,
                MyMessageBoxButtons.OK,
                this
            );
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm)
                vm.Password = (sender as PasswordBox)?.Password;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            WindowHelpers.DragWindow(this, e);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Close(this);
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Minimize(this);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.MaximizeOrRestore(this);
        }
    }
}
