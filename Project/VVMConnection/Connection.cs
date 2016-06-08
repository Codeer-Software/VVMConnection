using System.Windows;
using System.Linq;

namespace VVMConnection
{
    public class Connection
    {
        public static MethodCollection GetMethods(DependencyObject obj)=> (MethodCollection)obj.GetValue(MethodsProperty);

        public static void SetMethods(DependencyObject obj, MethodCollection value)=> obj.SetValue(MethodsProperty, value);

        public static readonly DependencyProperty MethodsProperty =
                DependencyProperty.RegisterAttached(
                    "Methods",
                    typeof(MethodCollection),
                    typeof(Connection),
                    new PropertyMetadata(null, Changed));

        static void Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            var col = sender.GetValue(MethodsProperty) as MethodCollection;
            col.ToList().ForEach(x => x.Connect(sender as FrameworkElement));
        }
    }
}