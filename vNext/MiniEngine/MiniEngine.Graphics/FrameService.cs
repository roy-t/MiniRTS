using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Skybox;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        public FrameService(GraphicsDevice device)
        {
            this.Skybox = null!;
            this.BrdfLutTexture = null!;
            this.Camera = new PerspectiveCamera(device.Viewport.AspectRatio);
            this.GBuffer = new GBuffer(device);
            this.LBuffer = new LBuffer(device);
            this.PBuffer = new PBuffer(device);
        }

        public ICamera Camera { get; set; }
        public GBuffer GBuffer { get; set; }
        public LBuffer LBuffer { get; set; }
        public PBuffer PBuffer { get; set; }

        public int GetBufferSize()
        {
            var gBufferSize = BufferSize(this.GBuffer.Depth, this.GBuffer.Diffuse, this.GBuffer.Material, this.GBuffer.Normal);
            var lBufferSize = BufferSize(this.LBuffer.Light);
            var lutSize = TextureSize(this.BrdfLutTexture);
            var skyboxSize = BufferSize(this.Skybox.Environment, this.Skybox.Irradiance, this.Skybox.Texture);

            return gBufferSize + lBufferSize + lutSize + skyboxSize;
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
                SurfaceFormat.Single => 4,
                SurfaceFormat.Vector2 => 8,
                SurfaceFormat.Vector4 => 32,
                SurfaceFormat.HalfVector2 => 4,
                SurfaceFormat.HalfVector4 => 8,
                SurfaceFormat.ColorSRgb => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };

        public SkyboxGeometry Skybox { get; set; } // TODO: this field should be move to a scene object and initialized better!

        public Texture2D BrdfLutTexture { get; set; } // TODO: this is a general shader resource that should be somewhere else?
    }
}
