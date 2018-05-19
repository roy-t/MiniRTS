using System.Collections.Generic;
using MiniEngine.Rendering.Systems;

namespace MiniEngine
{
    public sealed class SystemCollection
    {
        private int next = 1;
        private readonly List<Entity> Entities;

        public SystemCollection(
            ModelSystem modelSystem,
            AmbientLightSystem ambientLightSystem,
            SunlightSystem sunlightSystem,
            PointLightSystem pointLightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.Entities = new List<Entity>();

            this.ModelSystem = modelSystem;
            this.AmbientLightSystem = ambientLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.PointLightSystem = pointLightSystem;
            this.DirectionalLightSystem = directionalLightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

        public ModelSystem ModelSystem { get; }
        public AmbientLightSystem AmbientLightSystem { get; }
        public SunlightSystem SunlightSystem { get; }
        public PointLightSystem PointLightSystem { get; }
        public DirectionalLightSystem DirectionalLightSystem { get; }        
        public ShadowCastingLightSystem ShadowCastingLightSystem { get; }

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
                entities[i] = CreateEntity();
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
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

        private void RemoveEntityFromSystems(Entity entity)
        {
            this.ModelSystem.Remove(entity);
            this.SunlightSystem.Remove(entity);
            this.PointLightSystem.Remove(entity);
            this.DirectionalLightSystem.Remove(entity);
            this.ShadowCastingLightSystem.Remove(entity);            
        }
    }
}
