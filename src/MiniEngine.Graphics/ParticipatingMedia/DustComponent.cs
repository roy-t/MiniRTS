using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class DustComponent : AComponent
    {
        public DustComponent(Entity entity, GeometryData geometry, RenderTarget2D volumeBuffer, RenderTarget2D densityBuffer)
            : base(entity)
        {
            this.Geometry = geometry;
            this.VolumeBuffer = volumeBuffer;
            this.DensityBuffer = densityBuffer;
        }

        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeBuffer { get; }

        public RenderTarget2D DensityBuffer { get; }

        public static DustComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry)
        {
            var bufferA = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var bufferB = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            return new DustComponent(entity, geometry, bufferA, bufferB);
        }
    }
}
