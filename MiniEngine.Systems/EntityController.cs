using System;
using System.Collections.Generic;
using System.Text;

namespace MiniEngine.Systems
{
    public sealed class EntityController
    {
        private readonly EntityCreator Creator;
        private readonly IReadOnlyList<ISystem> Systems;

        public EntityController(EntityCreator creator, IEnumerable<ISystem> systems)
        {
            this.Creator = creator;
            this.Systems = new List<ISystem>(systems).AsReadOnly();
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

        public void DescribeAllEntities()
        {
            foreach(var entity in this.Creator.GetAllEntities())
            {
                this.DescribeEntity(entity);
            }
        }

        public string DescribeEntity(Entity entity)
        {
            var builder = new StringBuilder();
            builder.AppendLine(entity.ToString());

            foreach (var system in this.Systems)
            {
                if (system.Contains(entity))
                {
                    builder.Append('\t');
                    builder.AppendLine(system.Describe(entity));
                }
            }

            return builder.ToString();
        }

        private void RemoveEntityFromSystems(Entity entity)
        {
            foreach (var system in this.Systems)
            {
                system.Remove(entity);
            }
        }
    }
}
