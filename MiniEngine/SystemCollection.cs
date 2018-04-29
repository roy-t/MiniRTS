using System.Collections.Generic;
using MiniEngine.Rendering.Lighting.Systems;

namespace MiniEngine
{
    public sealed class SystemCollection
    {        
        private int next = 1;
        private readonly List<int> Entities;

        public SystemCollection(SunlightSystem sunlightSystem, PointLightSystem pointLightSystem)
        {            
            this.Entities = new List<int>();
            this.SunlightSystem = sunlightSystem;
            this.PointLightSystem = pointLightSystem;
        }

        public SunlightSystem SunlightSystem { get; }
        public PointLightSystem PointLightSystem { get; }

        public int CreateEntity()
        {
            var id = this.next++;
            this.Entities.Add(id);

            return id;
        }

        public void DestroyEntity(int entity)
        {
            this.Entities.Remove(entity);
            RemoveEntityFromSystems(entity);
        }        

        public void DestroyAllEntities()
        {
            foreach (var entity in this.Entities)
            {
                RemoveEntityFromSystems(entity);
            }

            this.Entities.Clear();
        }

        private void RemoveEntityFromSystems(int entity)
        {
            this.SunlightSystem.Remove(entity);
            this.PointLightSystem.Remove(entity);
        }
    }
}
