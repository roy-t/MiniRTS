using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Visibility
{
    [System]
    public partial class VisibilitySystem : ISystem
    {
        private FakeSpatialPartitioningStructure Partition;
        private readonly GeometryRenderService GeometryService;

        public VisibilitySystem(GeometryRenderService geometryService)
        {
            this.Partition = new FakeSpatialPartitioningStructure();
            this.GeometryService = geometryService;
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
