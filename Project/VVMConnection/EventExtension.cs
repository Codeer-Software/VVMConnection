using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace VVMConnection
{
    public class EventExtension : MarkupExtension
    {
        public string _path;

        public EventExtension(string path)
        {
            _path = path;
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

            Action invoke = () => target.DataContext.GetType().GetMethod(_path).Invoke(target.DataContext, new object[0]);
            var arguments = handlerType.GetMethod("Invoke").GetParameters();
            var conType = typeof(Core<,>).MakeGenericType(arguments[0].ParameterType, arguments[1].ParameterType);
            return conType.GetMethod("Connect").Invoke(null, new object[] { handlerType, invoke });
        }

        static class Core<O,E>
        {
            public class Connector
            {
                Action _core;
                public Connector(Action core) { _core = core; }
                public void Invoke(O o, E e) => _core();
            }

            public static object Connect(Type handlerType, Action core)
            {
                var connector = new Connector(core);
                return Delegate.CreateDelegate(handlerType, connector, connector.GetType().GetMethod("Invoke"));
            }
        }
    }
}
