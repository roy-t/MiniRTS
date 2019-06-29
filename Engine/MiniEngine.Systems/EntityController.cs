using System.Collections.Generic;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Systems
{
    public sealed class EntityController
    {
        private int next = 1;
        private readonly List<Entity> Entities;

        private readonly IReadOnlyList<ISystem> Systems;
        private readonly IReadOnlyList<IComponentFactory> Factories;

        public EntityController(IEnumerable<ISystem> systems, IEnumerable<IComponentFactory> factories)
        {
            this.Entities = new List<Entity>();
            this.Systems = new List<ISystem>(systems).AsReadOnly();
            this.Factories = new List<IComponentFactory>(factories).AsReadOnly();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this.next++);
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
            this.Entities.Remove(entity);
            this.RemoveEntityFromSystems(entity);
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
