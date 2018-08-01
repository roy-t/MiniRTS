using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MiniEngine.Rendering.Systems;
using MiniEngine.Systems;

namespace MiniEngine
{
    public sealed class SystemCollection
    {
        private int next = 1;
        private readonly List<Entity> Entities;
        private readonly List<ISystem> Systems;

        // TODO: look up the systems by the ISystem interface and use dependency injection
        // to hook-up their dependencies

        public SystemCollection(
            ModelSystem modelSystem,
            AmbientLightSystem ambientLightSystem,
            SunlightSystem sunlightSystem,
            PointLightSystem pointLightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.Entities = new List<Entity>();
            this.Systems = new List<ISystem>
            {
                modelSystem,
                ambientLightSystem,
                sunlightSystem,
                pointLightSystem,
                directionalLightSystem,
                shadowCastingLightSystem
            };

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

        public void DescribeAllEntities()
        {
            foreach (var entity in this.Entities)
            {
                Console.WriteLine(DescribeEntity(entity));
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
