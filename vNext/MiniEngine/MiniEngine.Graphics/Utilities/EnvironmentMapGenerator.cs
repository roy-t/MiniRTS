using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class EnvironmentMapGenerator
    {
        private const int resolution = 256;

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

            var cubeMap = new TextureCube(this.Device, resolution, true, SurfaceFormat.HalfVector4);

            var mipResolution = resolution;
            for (var mipMapLevel = 0; mipMapLevel < cubeMap.LevelCount; mipMapLevel++)
            {
                var roughness = mipMapLevel / (cubeMap.LevelCount - 1.0f);
                this.Effect.Roughness = roughness;

                CubeMapUtilities.RenderFaces(this.Device, cubeMap, this.Effect, mipResolution, SurfaceFormat.HalfVector4, mipMapLevel);
                mipResolution /= 2;
            }

            return cubeMap;
        }
    }
}