using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class ParticipatingMediaComponent : AComponent, IDisposable
    {
        private float quality = 1.0f;

        public ParticipatingMediaComponent(Entity entity, Color color, float strength, GeometryData geometry, RenderTarget2D volumeFrontBuffer, RenderTarget2D volumeBackBuffer, RenderTarget2D participatingMediaBuffer)
            : base(entity)
        {
            this.Color = color;
            this.Strength = strength;
            this.Geometry = geometry;
            this.VolumeFrontBuffer = volumeFrontBuffer;
            this.VolumeBackBuffer = volumeBackBuffer;
            this.ParticipatingMediaBuffer = participatingMediaBuffer;
            this.LightInfluence = 0.4f;
        }

        public float Strength { get; set; }

        public Color Color { get; set; }

        public float LightInfluence { get; set; }

        public float Quality { get => this.quality; set => this.ChangeQuality(value); }

        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeFrontBuffer { get; private set; }

        public RenderTarget2D VolumeBackBuffer { get; private set; }

        public RenderTarget2D ParticipatingMediaBuffer { get; private set; }

        public static ParticipatingMediaComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry, float quality, float strength, Color color)
        {
            var (front, back, media) = CreateRenderTargets(device, quality);
            return new ParticipatingMediaComponent(entity, color, strength, geometry, front, back, media);
        }

        public void Dispose()
        {
            this.VolumeFrontBuffer.Dispose();
            this.VolumeBackBuffer.Dispose();
            this.ParticipatingMediaBuffer.Dispose();
        }

        private void ChangeQuality(float quality)
        {
            this.quality = Math.Clamp(quality, 0.2f, 2.0f);

            var device = this.ParticipatingMediaBuffer.GraphicsDevice;

            this.Dispose();

            var (front, back, buffer) = CreateRenderTargets(device, this.quality);
            this.VolumeFrontBuffer = front;
            this.VolumeBackBuffer = back;
            this.ParticipatingMediaBuffer = buffer;
        }

        private static (RenderTarget2D front, RenderTarget2D back, RenderTarget2D media) CreateRenderTargets(GraphicsDevice device, float quality)
        {
            var width = (int)Math.Ceiling(device.PresentationParameters.BackBufferWidth * quality);
            var height = (int)Math.Ceiling(device.PresentationParameters.BackBufferHeight * quality);

            var front = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var back = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var media = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            return (front, back, media);
        }
    }
}
