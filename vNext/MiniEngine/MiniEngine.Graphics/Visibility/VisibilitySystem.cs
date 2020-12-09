using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Visibility
{
    [System]
    public partial class VisibilitySystem : ISystem
    {
        private FakeSpatialPartitioningStructure Partition;

        public VisibilitySystem()
        {
            this.Partition = new FakeSpatialPartitioningStructure();
        }

        public void OnSet()
        {
        }

        [ProcessAll]
        public void ComputeVisibleObjectsForCamera(CameraComponent camera)
        {
            camera.InView.Clear();
            this.Partition.GetVisibleGeometry(camera.Camera, camera.InView);
        }

        [ProcessNew]
        public void ProcessNew(TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Add(transform.Entity, geometry.Geometry, geometry.Material, transform.Matrix);

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Update(transform.Entity, transform.Matrix);

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, GeometryComponent geometry)
            => this.Partition.Remove(transform.Entity);
    }
}
