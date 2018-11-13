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
            var builder = new StringBuilder();
            var seen = new HashSet<Entity>();
            foreach (var entity in entities)
            {
                this.DescribeEntity(builder, seen, 0, entity);                
            }

            Console.WriteLine(builder.ToString());
        }

        public string DescribeEntity(Entity entity)
        {
            var builder = new StringBuilder();
            var seen = new HashSet<Entity>();

            this.DescribeEntity(builder, seen, 0, entity);

            return builder.ToString();
        }

        private void DescribeEntity(StringBuilder builder, HashSet<Entity> seen, int depth, Entity entity)
        {
            if (!seen.Contains(entity))
            {
                seen.Add(entity);

                builder.Append(new string('\t', depth));
                builder.AppendLine(entity.ToString());

                foreach (var system in this.Systems)
                {
                    if (system.Contains(entity))
                    {
                        builder.Append(new string('\t', depth + 1));
                        builder.AppendLine(system.Describe(entity));
                    }
                }

                var children = this.Creator.GetChilderen(entity);
                foreach (var child in children)
                {
                    this.DescribeEntity(builder, seen, depth + 1, child);
                }
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
