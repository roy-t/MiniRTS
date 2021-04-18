using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Shadows
{
    [System]
    public partial class ShadowMapSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly GeometryRenderService GeometryService;
        private readonly ShadowMapEffect Effect;

        private readonly RasterizerState RasterizerState;

        public ShadowMapSystem(GraphicsDevice device, GeometryRenderService geometryService, ShadowMapEffect effect)
        {
            this.Device = device;
            this.GeometryService = geometryService;
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

            var inView = camera.InView;
            for (var i = 0; i < inView.Count; i++)
            {
                var pose = inView[i];
                this.GeometryService.DrawToShadowMap(camera.Camera.ViewProjection, pose.Entity);
            }
        }
    }
}
