using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Shadows
{
    [System]
    public partial class ShadowMapSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly ShadowMapEffect Effect;

        private readonly RasterizerState RasterizerState;

        public ShadowMapSystem(GraphicsDevice device, ShadowMapEffect effect)
        {
            this.Device = device;
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

            for (var i = 0; i < camera.InView.Count; i++)
            {
                var pose = camera.InView[i];
                this.Draw(camera.Camera, pose.Geometry, pose.Transform);
            }
        }

        private void Draw(ICamera camera, GeometryData geometry, Matrix transform)
        {
            this.Effect.WorldViewProjection = transform * camera.ViewProjection;
            this.Effect.Apply();

            this.Device.SetVertexBuffer(geometry.VertexBuffer, 0);
            this.Device.Indices = geometry.IndexBuffer;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }
}
