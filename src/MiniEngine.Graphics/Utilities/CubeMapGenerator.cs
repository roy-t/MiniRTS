using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class CubeMapGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly CubeMapGeneratorEffect Effect;

        public CubeMapGenerator(GraphicsDevice device, CubeMapGeneratorEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Effect.EquirectangularTextureSampler = SamplerState.LinearClamp;
            this.Effect.EquirectangularTexture = equirectangularTexture;

            var resolution = equirectangularTexture.Height / 2;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4, this.Apply);
        }

        private void Apply(Matrix worldViewProjection)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Apply();
        }
    }
}
