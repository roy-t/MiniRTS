using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

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
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Effect.EquirectangularTexture = equirectangularTexture;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4);
        }
    }
}
