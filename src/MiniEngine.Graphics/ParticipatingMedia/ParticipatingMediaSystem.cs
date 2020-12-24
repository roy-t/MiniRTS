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

        public ParticipatingMediaSystem(GraphicsDevice device, LightPostProcessEffect effect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.Effect = effect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
        }

        [Process]
        public void Process()
        {
            this.Device.SetRenderTarget(this.FrameService.LBuffer.LightPostProcess);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            var camera = this.FrameService.CamereComponent.Camera;

            this.Effect.Light = this.FrameService.LBuffer.Light;
            this.Effect.Volume = this.FrameService.LBuffer.ParticipatingMedia;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);
            this.Effect.CameraPosition = camera.Position;
            this.Effect.FogColor = new Color(0.1f, 0.1f, 0.1f);
            this.Effect.Strength = 1.5f;

            this.Effect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
