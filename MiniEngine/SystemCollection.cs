using System.Collections.Generic;
using MiniEngine.Rendering.Lighting.Systems;

namespace MiniEngine
{
    public sealed class SystemCollection
    {
        private int next = 1;
        private readonly List<int> Entities;

        public SystemCollection(SunlightSystem sunlightSystem)
        {
            this.Entities = new List<int>();
            this.SunlightSystem = sunlightSystem;
        }

        public SunlightSystem SunlightSystem { get; }

        public int CreateEntity()
        {
            var id = this.next++;
            this.Entities.Add(id);

            return id;
        }

        public void DestroyEntity(int entity)
        {
            this.Entities.Remove(entity);

            this.SunlightSystem.Remove(entity);
        }
    }
}
