using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public static class SystemBinder
    {
        private static readonly string ProcessMethod = "Process";

        public static List<ISystemBinding> BindSystem(ISystemBase system, Dictionary<Type, IComponentContainer> componentContainers)
        {
            var systemBindings = new List<ISystemBinding>();
            var type = system.GetType();
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (typeof(ISystemBase).IsAssignableFrom(@interface) && @interface != typeof(ISystemBase))
                {
                    var method = @interface?.GetMethod(ProcessMethod);
                    var parameters = method?.GetParameters().Select(p => p.ParameterType).ToArray() ?? new Type[0];
                    var processor = type.GetMethod(ProcessMethod, parameters)!;

                    var arguments = @interface?.GetGenericArguments() ?? new Type[0];

                    if (arguments.Length == 0)
                    {
                        systemBindings.Add(new SystemBindingWithoutComponents(processor, system));
                    }
                    else if (arguments.Length == 1)
                    {
                        var parameterLookup = componentContainers[arguments[0]];
                        systemBindings.Add(new SystemBindingWithOneComponent(processor, system, parameterLookup));
                    }
                    else
                    {
                        var parameterLookups = new IComponentContainer[arguments.Length];
                        for (var i = 0; i < arguments.Length; i++)
                        {
                            parameterLookups[i] = componentContainers[arguments[i]];
                        }
                        systemBindings.Add(new SystemBindingWithManyComponents(processor, system, parameterLookups));
                    }

                }
            }

            return systemBindings;
        }
    }
}
