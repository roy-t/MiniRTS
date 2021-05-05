using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Graphics.Geometry
{
    [Service]
    public sealed class GeometryFactory
    {
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public GeometryFactory(EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Entities = entities;
            this.Components = components;
        }

        public (GeometryComponent, TransformComponent, BoundingSphereComponent) Create(GeometryModel model)
        {
            var entity = this.Entities.Create();

            var geometry = new GeometryComponent(entity, model);
            var transform = new TransformComponent(entity, Vector3.Zero, 1.0f);
            var bounds = new BoundingSphereComponent(entity, BoundingSphere.CreateMerged(new BoundingSphere(Vector3.Zero, 0.0f), model.Bounds).Radius);

            this.Components.Add(geometry, transform, bounds);

            return (geometry, transform, bounds);
        }

        public (GeometryComponent, TransformComponent, BoundingSphereComponent, InstancingComponent instances) Create(GeometryModel model, Matrix[] instances)
        {
            var entity = this.Entities.Create();

            var geometry = new GeometryComponent(entity, model);
            var transform = new TransformComponent(entity, Vector3.Zero, 1.0f);
            var bounds = new BoundingSphereComponent(entity, Merge(model.Bounds, instances).Radius);
            var instancing = InstancingComponent.Create(entity, instances);

            this.Components.Add(geometry, transform, bounds, instancing);

            return (geometry, transform, bounds, instancing);
        }

        private static BoundingSphere Merge(BoundingSphere root, Matrix[] transforms)
        {
            var merged = new BoundingSphere(Vector3.Zero, 0.0f);
            for (var i = 0; i < transforms.Length; i++)
            {
                var transformed = root.Transform(transforms[i]);
                merged = BoundingSphere.CreateMerged(merged, transformed);
            }

            return merged;
        }
    }
}
