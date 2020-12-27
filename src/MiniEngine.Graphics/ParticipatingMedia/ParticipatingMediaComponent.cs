using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class ParticipatingMediaComponent : AComponent, IDisposable
    {
        public ParticipatingMediaComponent(Entity entity, Color color, float strength, GeometryData geometry, RenderTarget2D volumeFrontBuffer, RenderTarget2D volumeBackBuffer, RenderTarget2D densityBuffer, RenderTarget2D participatingMediaBuffer)
            : base(entity)
        {
            this.Color = color;
            this.Strength = strength;
            this.Geometry = geometry;
            this.VolumeFrontBuffer = volumeFrontBuffer;
            this.VolumeBackBuffer = volumeBackBuffer;
            this.DensityBuffer = densityBuffer;
            this.ParticipatingMediaBuffer = participatingMediaBuffer;

        }

        public float Strength { get; set; }

        public Color Color { get; set; }

        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeFrontBuffer { get; }

        public RenderTarget2D VolumeBackBuffer { get; }

        public RenderTarget2D DensityBuffer { get; }

        public RenderTarget2D ParticipatingMediaBuffer { get; }

        public static ParticipatingMediaComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry, float strength, Color color)
        {
            // TODO: we might be able to use cheaper surface formats, like HalfSingle or maybe even Alpha8
            var front = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var back = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var density = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var buffer = new RenderTarget2D(device, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            return new ParticipatingMediaComponent(entity, color, strength, geometry, front, back, density, buffer);
        }

        public void Dispose()
        {
            this.VolumeFrontBuffer.Dispose();
            this.VolumeBackBuffer.Dispose();
            this.DensityBuffer.Dispose();
        }
    }
}
