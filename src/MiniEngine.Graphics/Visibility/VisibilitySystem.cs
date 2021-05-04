using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Visibility
{
    [System]
    public partial class VisibilitySystem : ISystem
    {
        private readonly FakeSpatialPartitioningStructure Partition;
        private readonly GeometryRenderService GeometryService;
        private readonly ParticleRenderService ParticleService;
        private readonly IComponentContainer<InstancingComponent> Instances;

        public VisibilitySystem(GeometryRenderService geometryService, ParticleRenderService particleService, IComponentContainer<InstancingComponent> instances)
        {
            this.Partition = new FakeSpatialPartitioningStructure();
            this.GeometryService = geometryService;
            this.ParticleService = particleService;
            this.Instances = instances;
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
        public void ProcessNew(TransformComponent transform, ParticleEmitterComponent emitter)
            => this.Partition.Add(transform.Entity, emitter.ComputeBounds(), transform.Transform, this.ParticleService);

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, ParticleEmitterComponent emitter)
            => this.Partition.Update(transform.Entity, emitter.ComputeBounds(), transform.Transform);

        [ProcessChanged]
        public void ProcessChanged(ParticleEmitterComponent emitter, TransformComponent transform)
            => this.Partition.Update(transform.Entity, emitter.ComputeBounds(), transform.Transform);

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, ParticleEmitterComponent _)
            => this.Partition.Remove(transform.Entity);

        // Models
        [ProcessNew]
        public void ProcessNew(TransformComponent transform, GeometryComponent geometry)
        {
            var isInstanced = this.Instances.Contains(geometry.Entity);
            this.Partition.Add(transform.Entity, geometry.Geometry.Bounds, transform.Transform, this.GeometryService, isInstanced);
        }

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Update(transform.Entity, geometry.Geometry.Bounds, transform.Transform);

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, GeometryComponent _)
            => this.Partition.Remove(transform.Entity);
    }
}
