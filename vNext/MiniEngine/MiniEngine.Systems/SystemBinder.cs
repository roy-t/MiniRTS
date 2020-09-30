using System;
using System.Collections.Generic;
using System.Reflection;
using MiniEngine.Configuration;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public static class SystemBinder
    {
        private static readonly string ProcessMethod = "Process";

        public static List<ISystemBinding> BindSystem(ISystem system, Resolve resolver, Dictionary<Type, IComponentContainer> componentContainers)
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
                    var containerIndices = new List<int>();

                    var services = new List<object>();
                    var serviceIndices = new List<int>();

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        if (componentContainers.TryGetValue(parameter.ParameterType, out var container))
                        {
                            containers.Add(container);
                            containerIndices.Add(i);
                        }
                        else
                        {
                            services.Add(resolver(parameter.ParameterType));
                            serviceIndices.Add(i);
                        }
                    }

                    systemBindings.Add(new SystemBinding(method, system, containers, containerIndices, services, serviceIndices));
                }
            }

            return systemBindings;
        }
    }
}
