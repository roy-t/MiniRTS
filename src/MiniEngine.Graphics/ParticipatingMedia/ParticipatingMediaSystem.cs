using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    [System]
    public partial class ParticipatingMediaSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly LightPostProcessEffect Effect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly FrameService FrameService;
        private readonly SamplerState ShadowMapSampler;

        public ParticipatingMediaSystem(GraphicsDevice device, LightPostProcessEffect effect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.Effect = effect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;

            this.ShadowMapSampler = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.SamplerStates[0] = this.ShadowMapSampler;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
        }

        [ProcessAll]
        public void Process(ParticipatingMediaComponent media, CascadedShadowMapComponent shadowMap)
        {
            this.Device.SetRenderTarget(this.FrameService.LBuffer.LightPostProcess);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            var camera = this.FrameService.CamereComponent.Camera;

            this.Effect.Light = this.FrameService.LBuffer.Light;
            this.Effect.Volume = media.DensityBuffer;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);
            this.Effect.CameraPosition = camera.Position;
            this.Effect.FogColor = new Color(0.1f, 0.1f, 0.1f);
            this.Effect.Strength = 4.0f;

            this.Effect.ShadowMap = shadowMap.DepthMapArray;
            this.Effect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.Effect.Splits = shadowMap.Splits;
            this.Effect.Offsets = shadowMap.Offsets;
            this.Effect.Scales = shadowMap.Scales;
            this.Effect.ViewDistance = camera.FarPlane;

            this.Effect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
