using System;
using System.Reflection;
using System.Windows;
using System.Linq;

namespace VVMConnection
{
    public class Method
    {
        public string Name { get; set; }
        public string Invoker { get; set; }
        public object Target { get; set; }

        public void Connect(FrameworkElement element)
        {
            if (element == null || Invoker == null || Name == null)
            {
                return;
            }
            var connect = MakeConnectAction(element);
            if (connect == null)
            {
                return;
            }
            connect();
            element.DataContextChanged += (_, __) => connect();
        }

        Action MakeConnectAction(FrameworkElement target)
        {
            return () =>
            {
                if (target.DataContext == null)
                {
                    return;
                }
                var invokerProp = target.DataContext.GetType().GetProperty(Invoker);
                if (invokerProp == null)
                {
                    return;
                }
                var method = GetMethod(target, invokerProp);
                if (method == null)
                {
                    return;
                }
                invokerProp.SetValue(target.DataContext, method);
            };
        }

        Delegate GetMethod(FrameworkElement element, PropertyInfo invokerProp)
        {
            object target = null;
            if (Target != null)
            {
                target = Target;
            }
            else if (element != null)
            {
                target = element;
            }
            else
            {
                return null;
            }

            var invokeInfo = invokerProp.PropertyType.GetMethod("Invoke");
            if (invokeInfo == null)
            {
                return null;
            }
            
            var targetMethodInfo = target.GetType().GetMethod(Name, invokeInfo.GetParameters().Select(e=>e.ParameterType).ToArray());
            if (targetMethodInfo == null)
            {
                return null;
            }
            return targetMethodInfo.CreateDelegate(invokerProp.PropertyType, target);
        }
    }
}
