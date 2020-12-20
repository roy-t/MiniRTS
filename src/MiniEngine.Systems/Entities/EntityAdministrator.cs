using System.Collections.Generic;
using System.Threading;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Entities
{
    [Service]
    public sealed class EntityAdministrator
    {
        private readonly List<Entity> Entities;
        private int nextId = 0;

        public EntityAdministrator()
        {
            this.Entities = new List<Entity>();
        }

        public Entity Create()
        {
            var id = Interlocked.Increment(ref this.nextId);
            var entity = new Entity(id);
            this.Entities.Add(entity);

            return entity;
        }

        public IReadOnlyList<Entity> GetAllEntities()
            => this.Entities.ToArray();

        public void Remove(Entity entity)
            => this.Entities.Remove(entity);
    }
}
