using System.Windows;

namespace VVMConnection
{
    public class MessageBox
    {
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
            //MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options
        }
    }
}
