using MiniEngine.Systems.Components;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class EntityLinker
    {
        private readonly ComponentLookup LookUp;

        public EntityLinker()
        {
            this.LookUp = new ComponentLookup();
        }

        public void AddComponent(Entity entity, IComponent component) => this.LookUp.AddComponent(entity, component);

        public void RemoveComponents<T>(Entity entity)
            where T : IComponent
            => this.LookUp.Remove(entity, typeof(T));            

        public void GetComponentsOfType<T>(IList<T> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(typeof(T));
            foreach(var component in components)
            {
                output.Add((T)component.Component);
            }            
        }

        public void GetComponentsOfEntity(Entity entity, IList<IComponent> output)
        {            
            var components = this.LookUp.Search(entity);
            foreach(var component in components)
            {
                output.Add(component.Component);
            }
        }                     
    }
}
