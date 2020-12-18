using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Entities
{
    [Service]
    public sealed class EntityAdministrator
    {
        private readonly ConcurrentBag<Entity> Entities;
        private int nextId = 0;

        public EntityAdministrator()
        {
            this.Entities = new ConcurrentBag<Entity>();
        }

        public Entity[] Copy() => this.Entities.ToArray();

        public Entity Create()
        {
            var id = Interlocked.Increment(ref this.nextId);
            var entity = new Entity(id);
            this.Entities.Add(entity);

            return entity;
        }

        public IReadOnlyList<Entity> GetAllEntities()
            => this.Entities.ToArray();
    }
}
