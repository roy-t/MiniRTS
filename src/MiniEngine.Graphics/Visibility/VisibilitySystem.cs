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

        [ProcessNew]
        public void ProcessNew(BoundingSphereComponent bounds, TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Add(transform.Entity, bounds.Radius, transform.Transform, this.GeometryService);

        [ProcessNew]
        public void ProcessNew(BoundingSphereComponent bounds, TransformComponent transform, ParticleEmitterComponent _)
            => this.Partition.Add(transform.Entity, bounds.Radius, transform.Transform, this.ParticleService);

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, BoundingSphereComponent bounds)
            => this.Partition.Update(transform.Entity, bounds.Radius, transform.Transform);

        [ProcessChanged]
        public void ProcessChanged(BoundingSphereComponent bounds, TransformComponent transform)
            => this.Partition.Update(transform.Entity, bounds.Radius, transform.Transform);

        [ProcessRemoved]
        public void ProcessRemoved(BoundingSphereComponent bounds)
            => this.Partition.Remove(bounds.Entity);
    }
}
