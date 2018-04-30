using System.Collections.Generic;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Rendering.Lighting.Systems;

namespace MiniEngine
{
    public sealed class SystemCollection
    {
        // TODO: do not directly use the scene in the RenderSystem but let the scene insert an entity 
        // that contains all the info needed to draw the meshes! 
        // This will make the render system just like any other ordinary system

        private int next = 1;
        private readonly List<Entity> Entities;

        public SystemCollection(
            SunlightSystem sunlightSystem,
            PointLightSystem pointLightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.Entities = new List<Entity>();
            this.SunlightSystem = sunlightSystem;
            this.PointLightSystem = pointLightSystem;
            this.DirectionalLightSystem = directionalLightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

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
            this.SunlightSystem.Remove(entity);
            this.PointLightSystem.Remove(entity);
            this.DirectionalLightSystem.Remove(entity);
            this.ShadowCastingLightSystem.Remove(entity);            
        }
    }
}
