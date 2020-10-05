using System;
using System.Collections.Generic;
using System.Reflection;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public static class SystemBinder
    {
        private static readonly string ProcessMethod = "Process";

        public static List<ISystemBinding> BindSystem(ISystem system, Dictionary<Type, IComponentContainer> componentContainers)
        {
            var systemBindings = new List<ISystemBinding>();
            var type = system.GetType();

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                if (ProcessMethod.Equals(method.Name))
                {
                    var parameters = method.GetParameters();

                    var containers = new List<IComponentContainer>();

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        if (componentContainers.TryGetValue(parameter.ParameterType, out var container))
                        {
                            containers.Add(container);
                        }
                        else
                        {
                            throw new ArgumentException($"Unexpected parameter {parameter.Name} in method {method.Name}");
                        }
                    }

                    systemBindings.Add(new SystemBinding(method, system, containers));
                }
            }

            return systemBindings;
        }
    }
}
