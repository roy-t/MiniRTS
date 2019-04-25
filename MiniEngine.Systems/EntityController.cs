using System.Collections.Generic;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Systems
{
    public sealed class EntityController
    {
        private readonly EntityCreator Creator;
        private readonly EntityLinker EntityLinker;
        private readonly IReadOnlyList<ISystem> Systems;
        private readonly IReadOnlyList<IComponentFactory> Factories;

        public EntityController(EntityCreator creator, EntityLinker entityLinker, IEnumerable<ISystem> systems, IEnumerable<IComponentFactory> factories)
        {
            this.Creator = creator;
            this.EntityLinker = entityLinker;
            this.Systems = new List<ISystem>(systems).AsReadOnly();
            this.Factories = new List<IComponentFactory>(factories).AsReadOnly();
        }

        public void DestroyEntity(Entity entity)
        {
            this.Creator.Remove(entity);
            this.RemoveEntityFromSystems(entity);
        }

        public void DestroyEntities(Entity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            {
                this.DestroyEntity(entities[i]);
            }
        }

        public void DestroyAllEntities()
        {
            var entities = this.Creator.GetAllEntities();
            for (var i = 0; i < entities.Count; i++)
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
