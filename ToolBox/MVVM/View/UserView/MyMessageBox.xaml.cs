using System.Windows;
using System.Windows.Media.Imaging;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class MyMessageBox : Window
    {
        public MyMessageBoxResult Result { get; private set; } = MyMessageBoxResult.None;

        public MyMessageBox(string title, string message, MyMessageBoxType type, MyMessageBoxButtons buttons)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            // Automatyczne dopasowanie wysokości do treści po załadowaniu
            this.Loaded += (s, e) =>
            {
                this.SizeToContent = SizeToContent.Height;
            };

            // Ikona zależnie od typu
            switch (type)
            {
                case MyMessageBoxType.Info:
                    IconImage.Source = new BitmapImage(new Uri("/Assets/info.png", UriKind.RelativeOrAbsolute));
                    break;
                case MyMessageBoxType.Error:
                    IconImage.Source = new BitmapImage(new Uri("/Assets/error.png", UriKind.RelativeOrAbsolute));
                    break;
                case MyMessageBoxType.Question:
                    IconImage.Source = new BitmapImage(new Uri("/Assets/question.png", UriKind.RelativeOrAbsolute));
                    break;
                case MyMessageBoxType.Success:
                    IconImage.Source = new BitmapImage(new Uri("/Assets/success.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    IconImage.Source = null;
                    break;
            }

            // Widoczność przycisków
            BtnOK.Visibility = Visibility.Collapsed;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnYes.Visibility = Visibility.Collapsed;
            BtnNo.Visibility = Visibility.Collapsed;

            switch (buttons)
            {
                case MyMessageBoxButtons.OK:
                    BtnOK.Visibility = Visibility.Visible;
                    break;
                case MyMessageBoxButtons.OKCancel:
                    BtnOK.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;
                case MyMessageBoxButtons.YesNo:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;
                case MyMessageBoxButtons.YesNoCancel:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Result = MyMessageBoxResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MyMessageBoxResult.Cancel;
            this.Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MyMessageBoxResult.Yes;
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MyMessageBoxResult.No;
            this.Close();
        }
    }
}