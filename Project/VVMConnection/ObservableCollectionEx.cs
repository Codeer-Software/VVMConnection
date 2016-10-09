using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace VVMConnection
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> values)
        {
            values.ToList().ForEach(e => Items.Add(e));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> values)
        {
            Items.Clear();
            AddRange(values);
        }
    }
}
