using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowText(string text)
        {
            System.Windows.MessageBox.Show(this, text);
        }
    }

    public class MessageBox
    {
        public object X { get; set; }
        public string Caption { get; set; } = string.Empty;
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public bool? Show(string message)
        {
            switch (System.Windows.MessageBox.Show(message, Caption, Button))
            {
                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                case MessageBoxResult.Cancel:
                    return false;
                default:
                    return null;
            }
        }
    }
}
