using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.User_Service;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        private User _user;

        // Zdarzenia, które View (Login.xaml.cs) może subskrybować:
        public event Action LoginSucceeded;
        public event Action<string> LoginFailed;

        internal LoginViewModel(IUserService loginService)
        {
            _userService = loginService ?? throw new ArgumentNullException(nameof(loginService));

            _user = new User();

            LoginCommand = new RelayCommand(_ => ExecuteLogin());
        }

        public ICommand LoginCommand { get; }

        public string UserName
        {
            get => _user.Login;
            set
            {
                _user.Login = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string Password
        {
            get => _user.Password;
            set
            {
                _user.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        // Dostęp do całego Usera
        internal User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        // Metoda wywoływana przez LoginCommand
        private async void ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(UserName) && string.IsNullOrWhiteSpace(Password))
            {
                MyMessageBoxService.Show(
                    "Błąd logowania",
                    "Nazwa użytkownika oraz hasło jest puste!",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                MyMessageBoxService.Show(
                    "Błąd logowania",
                    "Nazwa użytkownika jest pusta!",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                MyMessageBoxService.Show(
                    "Błąd logowania",
                    "Hasło jest puste!",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
                return;
            }

            var loggedUser = await _userService.GetUserByCredentialsAsync(UserName, Password);
            if (loggedUser != null)
            {
                Session.Set(loggedUser); // Ustawiamy użytkownika w sesji
                LoginSucceeded?.Invoke();
            }
            else
            {
                MyMessageBoxService.Show(
                    "Błąd logowania",
                    "Nieprawidłowa nazwa użytkownika lub hasło!",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }
    }
}

