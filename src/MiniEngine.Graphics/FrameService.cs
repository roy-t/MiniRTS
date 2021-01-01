using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        public FrameService(EntityAdministrator entities, ComponentAdministrator components, GraphicsDevice device)
        {
            this.Skybox = null!;
            this.GBuffer = new GBuffer(device);
            this.LBuffer = new LBuffer(device);
            this.PBuffer = new PBuffer(device);

            this.CamereComponent = new CameraComponent(entities.Create(), new PerspectiveCamera(device.Viewport.AspectRatio));
            components.Add(this.CamereComponent);
        }

        public CameraComponent CamereComponent { get; set; }
        public GBuffer GBuffer { get; set; }
        public LBuffer LBuffer { get; set; }
        public PBuffer PBuffer { get; set; }

        public SkyboxGeometry Skybox { get; set; } // TODO: this field should be replaced by a service that searches for the best skybox given the objects position

        public int GetBufferSize()
        {
            var gBufferSize = BufferSize(this.GBuffer.Depth, this.GBuffer.Albedo, this.GBuffer.Material, this.GBuffer.Normal);
            var lBufferSize = BufferSize(this.LBuffer.Light);
            var skyboxSize = BufferSize(this.Skybox.Environment, this.Skybox.Irradiance, this.Skybox.Texture);

            return gBufferSize + lBufferSize + skyboxSize;
        }

        private static int BufferSize(params TextureCube[] textures)
            => textures.Sum(x => TextureSize(x));

        private static int TextureSize(TextureCube texture)
        {
            var sum = 0;
            var size = texture.Size;
            for (var l = 0; l < texture.LevelCount; l++)
            {
                sum += size * size * FormatSizeInBytes(texture.Format) * 6;
                size /= 2;
            }

            return sum;
        }

        private static int BufferSize(params Texture2D[] textures)
            => textures.Sum(x => TextureSize(x));

        private static int TextureSize(Texture2D texture)
            => texture.Width * texture.Height * FormatSizeInBytes(texture.Format);

        private static int FormatSizeInBytes(SurfaceFormat format)
            => format switch
            {
                SurfaceFormat.Color => 4,
                SurfaceFormat.HalfSingle => 2,
                SurfaceFormat.Single => 4,
                SurfaceFormat.Vector2 => 8,
                SurfaceFormat.Vector4 => 32,
                SurfaceFormat.HalfVector2 => 4,
                SurfaceFormat.HalfVector4 => 8,
                SurfaceFormat.ColorSRgb => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };
    }
}
