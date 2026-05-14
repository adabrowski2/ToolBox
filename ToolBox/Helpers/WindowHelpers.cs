using System.Windows;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.Helpers
{
    public static class WindowHelpers
    {
        public static void Close(Window window)
        {
            if (window != null)
                window.Close();
        }

        public static void Minimize(Window window)
        {
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        public static void MaximizeOrRestore(Window window)
        {
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        public static void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is DependencyObject d)
            {
                Window.GetWindow(d)?.DragMove();
            }
        }
    }
}