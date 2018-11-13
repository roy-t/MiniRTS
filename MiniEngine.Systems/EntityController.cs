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
            var children = this.Creator.GetChilderen(entity);
            this.DestroyEntities(children);
            
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
            var entities = this.Creator.GetAllEntities();
            foreach (var entity in entities)
            {
                Console.WriteLine(this.DescribeEntity(entity));
            }
        }

        public string DescribeEntity(Entity entity)
        {
            var builder = new StringBuilder(entity.ToString());
            builder.AppendLine();

            this.DescribeEntity(builder, 1, entity);

            return builder.ToString();
        }

        private void DescribeEntity(StringBuilder builder, int depth, Entity entity)
        {
            foreach (var system in this.Systems)
            {
                if (system.Contains(entity))
                {
                    builder.Append(new string('\t', depth));
                    builder.AppendLine(system.Describe(entity));
                }
            }

            var children = this.Creator.GetChilderen(entity);
            foreach (var child in children)
            {
                this.DescribeEntity(builder, depth + 1, entity);
            }
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
