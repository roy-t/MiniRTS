using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Shadows
{
    [System]
    public partial class ShadowMapSystem : ISystem, IGeometryRendererUser<Matrix>
    {
        private readonly GraphicsDevice Device;
        private readonly GeometryRenderer Renderer;
        private readonly ShadowMapEffect Effect;

        private readonly RasterizerState RasterizerState;

        public ShadowMapSystem(GraphicsDevice device, GeometryRenderer renderer, ShadowMapEffect effect)
        {
            this.Device = device;
            this.Renderer = renderer;
            this.Effect = effect;

            this.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = this.RasterizerState;
        }

        [ProcessAll]
        public void Process(ShadowMapComponent shadowMap, CameraComponent camera)
        {
            this.Device.SetRenderTarget(shadowMap.DepthMap);
            this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            this.Renderer.Draw(camera.InView, this, camera.Camera.ViewProjection);
        }

        public void SetEffectParameters(Material material, Matrix transform, Matrix viewProjection)
        {
            this.Effect.WorldViewProjection = transform * viewProjection;

            this.Effect.Albedo = material.Albedo;
            this.Effect.MaskSampler = SamplerState.AnisotropicWrap;
        }

        public void ApplyEffect()
            => this.Effect.ApplyShadowMapTechnique();

        public void ApplyInstancedEffect()
            => this.Effect.ApplyInstancedShadowMapTechnique();
    }
}
