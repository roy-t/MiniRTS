using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class ParticipatingMediaComponent : AComponent, IDisposable
    {
        public ParticipatingMediaComponent(Entity entity, GeometryData geometry, RenderTarget2D volumeFrontBuffer, RenderTarget2D volumeBackBuffer, RenderTarget2D densityBuffer)
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

        public static ParticipatingMediaComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry)
        {
            var front = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var back = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var density = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            return new ParticipatingMediaComponent(entity, geometry, front, back, density);
        }

        public void Dispose()
        {
            this.VolumeFrontBuffer.Dispose();
            this.VolumeBackBuffer.Dispose();
            this.DensityBuffer.Dispose();
        }

        // TODO: dispose
    }
}
