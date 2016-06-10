using System;
using System.ComponentModel;
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
                return null;
            }
            var target = targetProvider.TargetObject as FrameworkElement;
            if (target == null)
            {
                return null;
            }
            return new CommandBridge(target, _method, _enableProperty);
        }

        class CommandBridge : ICommand
        {
            public event EventHandler CanExecuteChanged = (_,__)=> { };

            Action _invoke;
            Action<object> _invokeParam;
            dynamic _enable;

            internal CommandBridge(FrameworkElement target, string method, string enableProperty)
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
                    if (string.IsNullOrEmpty(enableProperty))
                    {
                        return;
                    }
                    var propInfo = vmType.GetProperty(enableProperty);
                    if (propInfo == null)
                    {
                        return;
                    }
                    _enable = propInfo.GetValue(vm);
                    if (_enable == null)
                    {
                        return;
                    }
                    Type type = _enable.GetType();
                    var propertyChanged = type.GetEvent("PropertyChanged");
                    if (propertyChanged == null || propertyChanged.EventHandlerType != typeof(PropertyChangedEventHandler))
                    {
                        return;
                    }
                    var valueProperty = type.GetProperty("Value");
                    if (valueProperty == null || valueProperty.PropertyType != typeof(bool))
                    {
                        return;
                    }
                    PropertyChangedEventHandler h = (_, __) => CanExecuteChanged(this, EventArgs.Empty);
                    _enable.PropertyChanged += h;
                };
            }
        }
    }
}
