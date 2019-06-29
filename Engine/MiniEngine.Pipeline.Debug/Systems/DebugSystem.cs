using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public abstract class DebugSystem : ISystem
    {
        private readonly List<IComponent> Components;
        private readonly IComponentContainer<DebugInfo> DebugInfos;
        private readonly IList<IComponentContainer> Containers;

        protected DebugSystem(IComponentContainer<DebugInfo> debugInfos, IList<IComponentContainer> containers)
        {
            this.DebugInfos = debugInfos;
            this.Containers = containers;
            this.Components = new List<IComponent>();
        }

        // Enumerates all entities that have a DebugInfo component, returns all property marked with the given attribute T
        protected IEnumerable<(Entity, IComponent, DebugInfo, PropertyInfo, T)> EnumerateAttributes<T>()
            where T : Attribute
        {
            for (var i = 0; i < this.DebugInfos.Count; i++)
            {
                var debugInfo = this.DebugInfos[i];
                for (var c = 0; c < this.Containers.Count; c++)
                {
                    var container = this.Containers[c];
                    for (var e = 0; e < container.Count; e++)
                    {
                        var component = container[e];
                        var entity = component.Entity;

                        if (entity == debugInfo.Entity)
                        {
                            var componentType = component.GetType();
                            var properties = componentType.GetProperties();
                            for (var p = 0; p < properties.Length; p++)
                            {
                                var property = properties[p];
                                var attribute = property.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();
                                if (attribute != null)
                                {
                                    yield return (entity, component, debugInfo, property, attribute);
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}
