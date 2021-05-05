using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    public sealed class ParticipatingMediaComponent : AComponent, IDisposable
    {
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

        public float Quality
        {
            get => this.ParticipatingMediaBuffer.Width / (float)this.ParticipatingMediaBuffer.GraphicsDevice.Viewport.Width;
            set => this.ChangeResolution(value);
        }


        public GeometryData Geometry { get; }

        public RenderTarget2D VolumeFrontBuffer { get; private set; }

        public RenderTarget2D VolumeBackBuffer { get; private set; }

        public RenderTarget2D ParticipatingMediaBuffer { get; private set; }

        public static ParticipatingMediaComponent Create(Entity entity, GraphicsDevice device, GeometryData geometry, int width, int height, float strength, Color color)
        {
            var (front, back, media) = CreateRenderTargets(device, width, height);
            return new ParticipatingMediaComponent(entity, color, strength, geometry, front, back, media);
        }

        public void Dispose()
        {
            this.VolumeFrontBuffer.Dispose();
            this.VolumeBackBuffer.Dispose();
            this.ParticipatingMediaBuffer.Dispose();
        }

        private void ChangeResolution(float quality)
        {
            var device = this.ParticipatingMediaBuffer.GraphicsDevice;
            quality = Math.Clamp(quality, 0.2f, 1.0f);

            this.Dispose();

            var width = (int)(device.Viewport.Width * quality);
            var height = (int)(device.Viewport.Height * quality);

            var (front, back, buffer) = CreateRenderTargets(device, width, height);
            this.VolumeFrontBuffer = front;
            this.VolumeBackBuffer = back;
            this.ParticipatingMediaBuffer = buffer;
        }

        private static (RenderTarget2D front, RenderTarget2D back, RenderTarget2D media) CreateRenderTargets(GraphicsDevice device, int width, int height)
        {
            var front = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var back = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            var media = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            return (front, back, media);
        }
    }
}
