using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Skybox
{
    [System]
    public sealed class SkyboxSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly SkyboxEffect Effect;

        public SkyboxSystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;

            this.Effect = effectFactory.Construct<SkyboxEffect>();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.DepthRead;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            // As an optimization we render the skybox last. Using the diffuse depth buffer we can cull most samples
            // which saves the cost of shading every pixel of the skybox
            this.Device.SetRenderTargets(this.FrameService.GBuffer.Diffuse, this.FrameService.LBuffer.Light);
        }

        public void Process()
        {
            var skybox = this.FrameService.Skybox;
            var camera = this.FrameService.Camera;
            var view = Matrix.CreateLookAt(Vector3.Zero, camera.Forward, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, camera.AspectRatio, 0.1f, 1.5f);

            this.Effect.Skybox = skybox.Texture;
            this.Effect.WorldViewProjection = view * projection;

            this.Effect.Apply();

            this.Device.SetVertexBuffer(skybox.VertexBuffer, 0);
            this.Device.Indices = skybox.IndexBuffer;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, skybox.Primitives);
        }
    }
}
