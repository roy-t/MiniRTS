using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class DustComponent : AComponent
    {
        public DustComponent(Entity entity, GeometryData geometry, RenderTarget2D volumeFrontBuffer, RenderTarget2D volumeBackBuffer, RenderTarget2D densityBuffer)
            : base(entity)
        {
            this.Geometry = geometry;
            this.VolumeFrontBuffer = volumeFrontBuffer;
            this.VolumeBackBuffer = volumeBackBuffer;
            this.DensityBuffer = densityBuffer;
        }

        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeFrontBuffer { get; }

        public RenderTarget2D VolumeBackBuffer { get; }

        public RenderTarget2D DensityBuffer { get; }

        public static DustComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry)
        {
            var front = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            var back = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            var density = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            return new DustComponent(entity, geometry, front, back, density);
        }

        // TODO: dispose
    }
}
