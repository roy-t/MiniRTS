using MiniEngine.Systems.Components;
using MiniEngine.Systems.Factories;
using System.Collections.Generic;
using System.Text;

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
            foreach (var entity in entities)
            {
                this.DestroyEntity(entity);
            }
        }

        public void DestroyAllEntities()
        {
            var entities = this.Creator.GetAllEntities();
            foreach (var entity in entities)
            {
                this.DestroyEntity(entity);
            }
        }

        public List<EntityDescription> DescribeAllEntities()
        {
            var entities = new List<EntityDescription>();
            var builder = new StringBuilder();

            foreach (var entity in this.Creator.GetAllEntities())
            {
                builder.Clear();

                entities.Add(new EntityDescription(entity, this.DescribeEntity(entity)));
            }

            return entities;
        }

        private List<ComponentDescription> DescribeEntity(Entity entity)
        {
            var components = new List<IComponent>();
            var descriptions = new List<ComponentDescription>();
            this.EntityLinker.GetComponents(entity, components);

            foreach (var component in components)
            {
                descriptions.Add(component.Describe());
            }

            return descriptions;
        }

        private void RemoveEntityFromSystems(Entity entity)
        {
            foreach (var factory in this.Factories)
            {
                factory.Deconstruct(entity);
            }
        }
    }
}
