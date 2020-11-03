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
            var gBufferSize = this.BufferSize(this.GBuffer.Depth, this.GBuffer.Diffuse, this.GBuffer.Material, this.GBuffer.Normal);
            var lBufferSize = this.BufferSize(this.LBuffer.Light);
            var pBufferSize = this.BufferSize(this.PBuffer.Combine, this.PBuffer.PostProcess);
            var lutSize = this.TextureSize(this.BrdfLutTexture);
            var skyboxSize = this.BufferSize(this.Skybox.Environment, this.Skybox.Irradiance, this.Skybox.Texture);

            return gBufferSize + lBufferSize + pBufferSize + lutSize + skyboxSize;
        }

        private int BufferSize(params TextureCube[] textures)
            => textures.Sum(x => this.TextureSize(x));

        private int TextureSize(TextureCube texture)
        {
            var sum = 0;
            var size = texture.Size;
            for (var l = 0; l < texture.LevelCount; l++)
            {
                sum += size * size * this.FormatSizeInBytes(texture.Format) * 6;
                size /= 2;
            }

            return sum;
        }

        private int BufferSize(params Texture2D[] textures)
            => textures.Sum(x => this.TextureSize(x));

        private int TextureSize(Texture2D texture)
            => texture.Width * texture.Height * this.FormatSizeInBytes(texture.Format);

        private int FormatSizeInBytes(SurfaceFormat format)
            => format switch
            {
                SurfaceFormat.Color => 4,
                SurfaceFormat.Single => 4,
                SurfaceFormat.Vector2 => 8,
                SurfaceFormat.Vector4 => 32,
                SurfaceFormat.HalfVector2 => 4,
                SurfaceFormat.HalfVector4 => 8,
                SurfaceFormat.ColorSRgb => 4,
                _ => throw new ArgumentOutOfRangeException(),
            };

        public SkyboxGeometry Skybox { get; set; } // TODO: this field should be move to a scene object and initialized better!

        public Texture2D BrdfLutTexture { get; set; } // TODO: this is a general shader resource that should be somewhere else?
    }
}
