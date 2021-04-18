using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Visibility
{
    [System]
    public partial class VisibilitySystem : ISystem
    {
        private readonly FakeSpatialPartitioningStructure Partition;
        private readonly GeometryRenderService GeometryService;
        private readonly ParticleRenderService ParticleService;

        public VisibilitySystem(GeometryRenderService geometryService, ParticleRenderService particleService)
        {
            this.Partition = new FakeSpatialPartitioningStructure();
            this.GeometryService = geometryService;
            this.ParticleService = particleService;
        }

        public void OnSet()
        {
        }

        [ProcessAll]
        public void ComputeVisibleObjectsForCamera(CameraComponent camera)
        {
            camera.InView.Clear();
            this.Partition.GetVisibleEntities(camera.Camera, camera.InView);
        }

        // Particles
        [ProcessNew]
        public void ProcessNew(TransformComponent transform, ParticleEmitterComponent _)
            => this.Partition.Add(transform.Entity, new BoundingSphere(), transform.Transform, this.ParticleService);

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, ParticleEmitterComponent _)
            => this.Partition.Update(transform.Entity, transform.Transform);

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, ParticleEmitterComponent _)
            => this.Partition.Remove(transform.Entity);

        // Models
        [ProcessNew]
        public void ProcessNew(TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Add(transform.Entity, geometry.Geometry.Bounds, transform.Transform, this.GeometryService);

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, GeometryComponent _)
            => this.Partition.Update(transform.Entity, transform.Transform);

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, GeometryComponent _)
            => this.Partition.Remove(transform.Entity);
    }
}
