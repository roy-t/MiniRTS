using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class ParticipatingMediaComponent : AComponent, IDisposable
    {
        private float quality = 0.5f;

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

        public float Quality { get => this.quality; set => ChangeQuality(value); }

        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeFrontBuffer { get; private set; }

        public RenderTarget2D VolumeBackBuffer { get; private set; }

        public RenderTarget2D DensityBuffer { get; private set; }

        public RenderTarget2D ParticipatingMediaBuffer { get; private set; }

        public static ParticipatingMediaComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry, float quality, float strength, Color color)
        {
            var (front, back, density, media) = CreateRenderTargets(device, quality);
            return new ParticipatingMediaComponent(entity, color, strength, geometry, front, back, density, media);
        }

        public void Dispose()
        {
            this.VolumeFrontBuffer.Dispose();
            this.VolumeBackBuffer.Dispose();
            this.DensityBuffer.Dispose();
            this.ParticipatingMediaBuffer.Dispose();
        }

        private void ChangeQuality(float quality)
        {
            this.quality = Math.Clamp(quality, 0.2f, 2.0f);

            var device = this.ParticipatingMediaBuffer.GraphicsDevice;

            this.Dispose();

            var (front, back, density, buffer) = CreateRenderTargets(device, this.quality);
            this.VolumeFrontBuffer = front;
            this.VolumeBackBuffer = back;
            this.DensityBuffer = density;
            this.ParticipatingMediaBuffer = buffer;
        }

        private static (RenderTarget2D front, RenderTarget2D back, RenderTarget2D density, RenderTarget2D media) CreateRenderTargets(GraphicsDevice device, float quality)
        {
            var width = (int)Math.Ceiling(device.PresentationParameters.BackBufferWidth * quality);
            var height = (int)Math.Ceiling(device.PresentationParameters.BackBufferHeight * quality);

            // TODO: we might be able to use cheaper surface formats, like HalfSingle or maybe even Alpha8
            var front = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var back = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var density = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var media = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            return (front, back, density, media);
        }
    }
}
