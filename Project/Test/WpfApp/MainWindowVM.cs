using Reactive.Bindings;
using System;
using System.Reactive.Linq;

namespace WpfApp
{
    public class MainWindowVM
    {
        public ReactiveProperty<string> Number { get; } = new ReactiveProperty<string>("0");
        public ReactiveProperty<bool> CanCalculate { get; }
        public ReactiveProperty<string> Answer { get; } = new ReactiveProperty<string>();
        public Action<string> NotifyText { get; set; }
        public Func<string, bool?> Ask { get; set; }

        public MainWindowVM()
        {
            int val = 0;
            CanCalculate = Number.Select(x => int.TryParse(x, out val)).ToReactiveProperty();
        }

        public void HelloWorld()
        {
            NotifyText("HelloWorld");
        }

        public void Calculate()
        {
            bool? ret = Ask("Do you want to know the answer?");
            if (ret != true)
            {
                return;
            }
            Answer.Value = (int.Parse(Number.Value) * 2).ToString();
        }
    }
}
