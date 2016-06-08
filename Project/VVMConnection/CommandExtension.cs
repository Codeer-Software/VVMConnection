using Reactive.Bindings;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace VVMConnection
{
    public class CommandExtension : MarkupExtension
    {
        string _method;
        string _enableProperty;

        public CommandExtension(string method, string enableProperty)
        {
            _method = method;
            _enableProperty = enableProperty;
        }

        public CommandExtension(string method)
        {
            _method = method;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProvider == null)
            {
                return new CommandEmpty();
            }
            var target = targetProvider.TargetObject as FrameworkElement;
            if (target == null)
            {
                return new CommandEmpty();
            }
            return new CommandBridge(target, _method, _enableProperty);
        }

        public class CommandEmpty : ICommand
        {
            public event EventHandler CanExecuteChanged = (_, __) => { };
            public bool CanExecute(object parameter) { return false; }
            public void Execute(object parameter) { }
        }

        public class CommandBridge : ICommand
        {
            public event EventHandler CanExecuteChanged = (_,__)=> { };

            Action _invoke;
            Action<object> _invokeParam;
            ReactiveProperty<bool> _enable;

            public CommandBridge(FrameworkElement target, string method, string enableProperty)
            {
                var connect = MakeConnectAction(target, method, enableProperty);
                connect();
                target.DataContextChanged += (_, __) => connect();
            }

            public bool CanExecute(object parameter)=> _enable == null ? true : _enable.Value;

            public void Execute(object parameter)
            {
                if (_invoke != null) _invoke();
                else _invokeParam(parameter);
            }

            Action MakeConnectAction(FrameworkElement target, string method, string enableProperty)
            {
                return () =>
                {
                    var vm = target.DataContext;
                    if (vm == null)
                    {
                        return;
                    }
                    var vmType = vm.GetType();
                    var methodInfo = vmType.GetMethod(method);
                    var args = methodInfo.GetParameters();
                    if (args.Length == 0)
                    {
                        _invoke = () => methodInfo.Invoke(vm, new object[0]);
                    }
                    else if (args.Length == 1 && args[0].ParameterType == typeof(object))
                    {
                        _invokeParam = p => methodInfo.Invoke(vm, new object[] { p });
                    }
                    else
                    {
                        return;
                    }
                    if (!string.IsNullOrEmpty(enableProperty))
                    {
                        var propInfo = vmType.GetProperty(enableProperty);
                        if (propInfo != null && propInfo.PropertyType == typeof(ReactiveProperty<bool>))
                        {
                            _enable = propInfo.GetValue(vm) as ReactiveProperty<bool>;
                            _enable.PropertyChanged += (_, __) => CanExecuteChanged(this, EventArgs.Empty);
                        }
                    }
                };
            }

        }
    }
}
