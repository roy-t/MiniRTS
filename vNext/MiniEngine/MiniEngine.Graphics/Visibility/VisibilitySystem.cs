using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Visibility
{
    [System]
    public partial class VisibilitySystem : ISystem
    {
        public void OnSet()
        {
        }

        [ProcessChanged]
        public void ProcessChanged(TransformComponent transform, GeometryComponent geometry)
        {
        }

        [ProcessNew]
        public void ProcessNew(TransformComponent transform, GeometryComponent geometry)
        {
        }

        [ProcessRemoved]
        public void ProcessRemoved(TransformComponent transform, GeometryComponent geometry)
        {
        }
    }
}
