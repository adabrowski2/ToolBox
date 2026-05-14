using System.Windows;
using Praca_Inżynierska_v1.MVVM.View;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.Services
{
    public static class MyMessageBoxService
    {
        public static MyMessageBoxResult Show(
            string title,
            string message,
            MyMessageBoxType type = MyMessageBoxType.Info,
            MyMessageBoxButtons buttons = MyMessageBoxButtons.OK,
            Window owner = null
        )
        {
            var box = new MyMessageBox(title, message, type, buttons);

            if (owner != null)
            {
                box.Owner = owner;
            }

            box.ShowDialog();
            return box.Result;
        }
    }
}


