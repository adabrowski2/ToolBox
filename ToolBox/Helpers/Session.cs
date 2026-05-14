using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Helpers
{
    internal static class Session
    {
        public static User CurrentUser { get; private set; }

        public static string Role => CurrentUser?.Role;

        public static void Set(User user)
        {
            CurrentUser = user;
        }

        public static void Clear()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn => CurrentUser != null;
    }
}
