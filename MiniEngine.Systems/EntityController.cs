using System;
using System.Collections.Generic;
using System.Text;

namespace MiniEngine.Systems
{
    public sealed class EntityController
    {
        private int next = 1;
        private readonly List<Entity> Entities;
        private readonly IReadOnlyList<ISystem> Systems;

        public EntityController(IEnumerable<ISystem> systems)
        {
            this.Entities = new List<Entity>();
            this.Systems = new List<ISystem>(systems).AsReadOnly();
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

        public void DestroyEntity(Entity entity)
        {
            this.Entities.Remove(entity);
            this.RemoveEntityFromSystems(entity);
        }

        public void DestroyAllEntities()
        {
            foreach (var entity in this.Entities)
            {
                this.RemoveEntityFromSystems(entity);
            }

            this.Entities.Clear();
        }

        public void DescribeAllEntities()
        {
            foreach (var entity in this.Entities)
            {
                Console.WriteLine(this.DescribeEntity(entity));
            }
        }

        public string DescribeEntity(Entity entity)
        {
            var builder = new StringBuilder(entity.ToString());
            builder.AppendLine();
            foreach (var system in this.Systems)
            {
                if (system.Contains(entity))
                {
                    builder.Append("\t - ");
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
