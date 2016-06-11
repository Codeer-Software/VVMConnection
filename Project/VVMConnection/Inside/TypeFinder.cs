using System;
using System.Collections.Generic;
using System.Reflection;

namespace VVMConnection.Inside
{
    public static class TypeFinder
    {
        static Dictionary<string, Type> _fullNameAndType = new Dictionary<string, Type>();

        public static Type GetType(string typeFullName)
        {
            lock (_fullNameAndType)
            {
                Type type = null;
                if (_fullNameAndType.TryGetValue(typeFullName, out type))
                {
                    return type;
                }
			
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<Type> assemblyTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    type = assembly.GetType(typeFullName);
                    if (type != null)
                    {
                        break;
                    }
                }
                if (type != null)
                {
                    _fullNameAndType.Add(typeFullName, type);
                }
                return type;
            }
        }
    }
}