using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class EnvironmentMapGenerator
    {
        private const int resolution = 32;

        private readonly GraphicsDevice Device;
        private readonly EnvironmentMapGeneratorEffect Effect;

        public EnvironmentMapGenerator(GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<EnvironmentMapGeneratorEffect>();
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;


            this.Effect.EquirectangularTexture = equirectangularTexture;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4);
        }
    }
}