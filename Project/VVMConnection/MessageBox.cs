using System.Windows;

namespace VVMConnection
{
    public class MessageBox
    {
        public string MessageBoxText { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;
        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.None;
        public MessageBoxOptions Options { get; set; } = MessageBoxOptions.None;

        public bool? Show()
        {
            return Show(MessageBoxText);
        }

        public bool? Show(string messageBoxText)
        {
            switch (System.Windows.MessageBox.Show(messageBoxText, Caption, Button, Icon, DefaultResult, Options))
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
