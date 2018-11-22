using MiniEngine.Systems.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Systems
{
    public sealed class EntityLinker
    {
        private readonly MultiValueDictionary<Entity, IComponent> EntityComponents;
        private readonly MultiValueDictionary<Type, IComponent> TypeComponents;        

        public EntityLinker()
        {
            this.EntityComponents = new MultiValueDictionary<Entity, IComponent>();
            this.TypeComponents = new MultiValueDictionary<Type, IComponent>();
        }

        public void GetComponentsOfType<T>(IList<T> output)
            where T : IComponent
        {
            var components = this.TypeComponents.Get(typeof(T));
            foreach(var component in components)
            {
                output.Add((T)component);
            }            
        }

        public IEnumerable<T> GetComponents<T>(Entity entity) 
            where T : IComponent
            => this.EntityComponents.Get(entity).Where(x => x.GetType() == typeof(T)).Select(x => (T)x);
        
        public T GetComponent<T>(Entity entity)
            where T : IComponent
            => (T)this.EntityComponents.Get(entity).First(x => x.GetType() == typeof(T));

        public IEnumerable<IComponent> GetAllComponents(Entity entity)
            => this.EntityComponents.Get(entity);
        
        public void AddComponent<T>(Entity entity, T component)
            where T : IComponent
        {
            this.EntityComponents.Add(entity, component);
            this.TypeComponents.Add(typeof(T), component);
        }   
        
        public void RemoveComponents<T>(Entity entity)
        {
            var type = typeof(T);

            var entityComponents = this.EntityComponents.Get(entity);
            var typeComponents = this.TypeComponents.Get(type);

            var toRemove = entityComponents.Where(x => x.GetType() == type).ToList();
            
            foreach(var item in toRemove)
            {
                entityComponents.Remove(item);
                typeComponents.Remove(item);
            }
        }        
    }
}
