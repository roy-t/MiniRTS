using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class EnvironmentMapGenerator
    {
        private const int resolution = 512;

        private readonly GraphicsDevice Device;
        private readonly EnvironmentMapGeneratorEffect Effect;

        public EnvironmentMapGenerator(GraphicsDevice device, EnvironmentMapGeneratorEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            // I don't seem to have problems with 'Pre-filter convolution artifacts' but in case of
            // problems check that section: https://learnopengl.com/PBR/IBL/Specular-IBL

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

                CubeMapUtilities.RenderFaces(this.Device, cubeMap, this.Effect, mipResolution, SurfaceFormat.HalfVector4, mipMapLevel, this.Apply);
                mipResolution /= 2;
            }

            return cubeMap;
        }

        private void Apply(Matrix worldViewProjection)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Apply();
        }
    }
}
