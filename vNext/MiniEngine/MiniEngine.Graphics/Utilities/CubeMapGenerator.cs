using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class CubeMapGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly CubeMapGeneratorEffect Effect;

        public CubeMapGenerator(GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<CubeMapGeneratorEffect>();
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            var resolution = equirectangularTexture.Height / 2;
            this.Effect.EquirectangularTexture = equirectangularTexture;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4);
        }
    }
}
