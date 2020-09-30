using System.Collections.Concurrent;
using System.Threading;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Entities
{
    [Service]
    public sealed class EntityAdministrator
    {
        private readonly ConcurrentBag<Entity> Entities;
        private int nextId = 1;

        public EntityAdministrator()
        {
            this.Entities = new ConcurrentBag<Entity>();
        }

        public Entity Create()
        {
            var id = Interlocked.Increment(ref this.nextId);
            var entity = new Entity(id);
            this.Entities.Add(entity);

            return entity;
        }
    }
}
