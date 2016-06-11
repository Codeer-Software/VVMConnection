using System.ComponentModel;

namespace VVMConnection
{
    public class Notify<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };

        T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public Notify() { }
        public Notify(T value) { _value = value; }
    }
}
