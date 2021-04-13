using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class IrradianceMapGenerator
    {
        private const int resolution = 32;

        private readonly GraphicsDevice Device;
        private readonly IrradianceMapGeneratorEffect Effect;

        public IrradianceMapGenerator(GraphicsDevice device, IrradianceMapGeneratorEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Effect.EquirectangularTexture = equirectangularTexture;
            this.Effect.EquirectangularTextureSampler = SamplerState.LinearClamp;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4, this.Apply);
        }

        private void Apply(Matrix worldViewProjection)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Apply();
        }
    }
}
