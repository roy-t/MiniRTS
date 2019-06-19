using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public abstract class DebugSystem : ISystem
    {
        private readonly List<Entity> Entities;
        private readonly List<IComponent> Components;
        private readonly EntityLinker Linker;

        protected DebugSystem(EntityLinker linker)
        {
            this.Entities = new List<Entity>();
            this.Components = new List<IComponent>();
            this.Linker = linker;
        }

        // Enumerates all entities that have a DebugInfo component, returns all property marked with the given attribute T
        protected IEnumerable<(Entity, IComponent, DebugInfo, PropertyInfo, T)> EnumerateAttributes<T>()
            where T : Attribute
        {
            this.Entities.Clear();
            this.Linker.GetEntities<DebugInfo>(this.Entities);

            foreach (var entity in this.Entities)
            {
                var info = this.Linker.GetComponent<DebugInfo>(entity);

                this.Components.Clear();
                this.Linker.GetComponents(entity, this.Components);

                foreach (var component in this.Components)
                {
                    var componentType = component.GetType();

                    foreach (var property in componentType.GetProperties())
                    {
                        var attribute = property.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();
                        if (attribute != null)
                        {
                            yield return (entity, component, info, property, attribute);
                        }
                    }
                }
            }
        }
    }
}
