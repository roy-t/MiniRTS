using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class EnvironmentMapGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly EnvironmentMapGeneratorEffect Effect;

        public EnvironmentMapGenerator(GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<EnvironmentMapGeneratorEffect>();
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            var resolution = equirectangularTexture.Height / 2;
            this.Effect.EquirectangularTexture = equirectangularTexture;
            return CubeMapUtilities.RenderFaces(this.Device, this.Effect, resolution, SurfaceFormat.HalfVector4);
        }
    }
}