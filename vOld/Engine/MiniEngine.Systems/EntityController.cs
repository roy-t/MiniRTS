using System.Collections.Generic;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Systems
{
    public sealed class EntityController
    {
        private int next = 1;
        private readonly List<Entity> Entities;

        private readonly IReadOnlyList<IComponentFactory> Factories;
        private readonly IComponentContainer<Parent> Parents;

        public EntityController(IEnumerable<IComponentFactory> factories, IComponentContainer<Parent> parents)
        {
            this.Entities = new List<Entity>();
            this.Factories = new List<IComponentFactory>(factories).AsReadOnly();
            this.Parents = parents;
        }

        public Entity CreateEntity() => this.CreateEntity(string.Empty);

        public Entity CreateEntity(string name)
        {
            var entity = new Entity(this.next++, name);
            this.Entities.Add(entity);

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = new Entity[count];
            for (var i = 0; i < count; i++)
            {
                entities[i] = this.CreateEntity();
            }

            return entities;
        }

        public IReadOnlyList<Entity> GetAllEntities() => this.Entities.AsReadOnly();

        public void DestroyEntity(Entity entity)
        {
            var toDestroy = new Stack<Entity>(1);
            toDestroy.Push(entity);

            if (this.Parents.TryGet(entity, out var parent))
            {
                for (var i = 0; i < parent.Children.Count; i++)
                {
                    toDestroy.Push(parent.Children[i]);
                }
            }

            while (toDestroy.Count > 0)
            {
                var current = toDestroy.Pop();
                this.Entities.Remove(current);
                this.RemoveEntityFromSystems(current);
            }
        }

        public void DestroyAllEntities()
        {
            var entities = this.GetAllEntities();
            for (var i = entities.Count - 1; i >= 0; i--)
            {
                this.DestroyEntity(entities[i]);
            }
        }

        private void RemoveEntityFromSystems(Entity entity)
        {
            for (var i = 0; i < this.Factories.Count; i++)
            {
                this.Factories[i].Deconstruct(entity);
            }
        }
    }
}
