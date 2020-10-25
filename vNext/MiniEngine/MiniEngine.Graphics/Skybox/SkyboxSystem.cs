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
            this.Device.DepthStencilState = DepthStencilState.Default; // TODO: either clear the depth buffer after, or don't do depth
            this.Device.RasterizerState = RasterizerState.CullNone; // TODO: Cull
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.GBuffer.Diffuse);
        }

        public void Process(SkyboxComponent skybox)
        {
            var camera = this.FrameService.Camera;
            var view = Matrix.CreateLookAt(Vector3.Zero, camera.Forward, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, camera.AspectRatio, 0.1f, 250.0f);

            this.Effect.Skybox = skybox.Texture;
            this.Effect.WorldViewProjection = camera.ViewProjection; //view * projection;

            this.Effect.Apply();

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, skybox.Vertices, 0, skybox.Vertices.Length, skybox.Indices, 0, skybox.Primitives);
        }
    }
}
