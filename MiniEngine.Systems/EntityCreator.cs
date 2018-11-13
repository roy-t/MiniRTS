using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class EntityCreator
    {  
        private int next = 1;
        private readonly List<Entity> Entities;
        private readonly Dictionary<Entity, Entity[]> ChildEntities;

        public EntityCreator()
        {
            this.Entities = new List<Entity>();
            this.ChildEntities = new Dictionary<Entity, Entity[]>();
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

        public Entity[] CreateChildEntities(Entity parent, int count)
        {
            var children = this.CreateEntities(count);
            this.ChildEntities.Add(parent, children);
            return children;
        }

        public IReadOnlyList<Entity> GetAllEntities() => this.Entities.AsReadOnly();

        public Entity[] GetChilderen(Entity entity)
        {
            if(this.ChildEntities.TryGetValue(entity, out var children))
            {
                return children;
            }

            return new Entity[0];
        }

        public void Remove(Entity entity)
        {
            this.Entities.Remove(entity);
            if (this.ChildEntities.TryGetValue(entity, out var children))
            {
                foreach(var child in children)
                {
                    this.Remove(child);
                }
            }
        }
    }
}
