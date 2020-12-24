using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class DustComponent : AComponent
    {
        public DustComponent(Entity entity, GeometryData geometry)
            : base(entity)
        {
            this.Geometry = geometry;
        }

        public GeometryData Geometry { get; }
    }
}
