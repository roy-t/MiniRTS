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

        public string DescribeAllEntities()
        {
            var builder = new StringBuilder();
            foreach(var entity in this.Creator.GetAllEntities())
            {
                this.DescribeEntity(builder, entity);
            }

            return builder.ToString();
        }

        public string DescribeEntity(Entity entity) => this.DescribeEntity(new StringBuilder(), entity);

        private string DescribeEntity(StringBuilder builder, Entity entity)
        {            
            builder.AppendLine(entity.ToString());

            foreach(var component in this.EntityLinker.GetAllComponents(entity))
            {
                builder.Append('\t');
                builder.AppendLine(component.ToString());
            }
            
            return builder.ToString();
        }

        private void RemoveEntityFromSystems(Entity entity)
        {
            foreach(var factory in this.Factories)
            {
                factory.Deconstruct(entity);
            }
        }
    }
}
