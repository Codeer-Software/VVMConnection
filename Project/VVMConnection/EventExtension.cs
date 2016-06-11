using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Linq;
using VVMConnection.Inside;

namespace VVMConnection
{
    public class EventExtension : MarkupExtension
    {
        public string _path;
        public string _bridge;

        public EventExtension(string path)
        {
            _path = path;
        }

        public EventExtension(string path, string bridge)
        {
            _path = path;
            _bridge = bridge;
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

            Type handlerType = null;
            var eventInfo = targetProvider.TargetProperty as EventInfo;
            if (eventInfo != null)
            {
                handlerType = eventInfo.EventHandlerType;
            }
            else
            {
                var method = targetProvider.TargetProperty as MethodInfo;
                handlerType = method.GetParameters()[1].ParameterType;
            }

            Action<object, object> invoke = null;
            if (string.IsNullOrEmpty(_bridge))
            {
                invoke = (_, __) => target.DataContext.GetType().GetMethod(_path).Invoke(target.DataContext, new object[0]);
            }
            else
            {
                var items = _bridge.Split('.');
                if (items.Length < 2)
                {
                    return null;
                }
                var bridgeType = TypeFinder.GetType(string.Join(".", items.Take(items.Length - 1)));
                if (bridgeType == null)
                {
                    return null;
                }
                var methodInfo = bridgeType.GetMethod(items[items.Length - 1]);
                var args = methodInfo.GetParameters();
                if (args.Length != 3 && typeof(Delegate).IsAssignableFrom(args[2].ParameterType))
                {
                    return null;
                }
                invoke = (o, e) => methodInfo.Invoke(null,
                    new object[] { o, e, target.DataContext.GetType().GetMethod(_path).CreateDelegate(args[2].ParameterType, target.DataContext) });
            }

            var arguments = handlerType.GetMethod("Invoke").GetParameters();
            var conType = typeof(Core<,>).MakeGenericType(arguments[0].ParameterType, arguments[1].ParameterType);
            return conType.GetMethod("Connect").Invoke(null, new object[] { handlerType, invoke });
        }

        static class Core<O,E>
        {
            public class Connector
            {
                Action<object, object> _core;
                public Connector(Action<object, object> core) { _core = core; }
                public void Invoke(O o, E e) => _core(o, e);
            }

            public static object Connect(Type handlerType, Action<object, object> core)
            {
                var connector = new Connector(core);
                return Delegate.CreateDelegate(handlerType, connector, connector.GetType().GetMethod("Invoke"));
            }
        }
    }
}
