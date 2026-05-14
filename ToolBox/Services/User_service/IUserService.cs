using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.User_Service
{
    internal interface IUserService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<User> GetUserByCredentialsAsync(string login, string password);
    }

}

