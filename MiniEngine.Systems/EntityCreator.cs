using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class EntityCreator
    {  
        private int next = 1;
        private readonly List<Entity> Entities;

        public EntityCreator()
        {
            this.Entities = new List<Entity>();
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

        public void Remove(Entity entity) => this.Entities.Remove(entity);
    }
}
