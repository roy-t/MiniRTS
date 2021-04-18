using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public partial class RenderSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;

        public RenderSystem(GraphicsDevice device, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTargets(
                this.FrameService.GBuffer.Albedo,
                this.FrameService.GBuffer.Material,
                this.FrameService.GBuffer.Depth,
                this.FrameService.GBuffer.Normal);
        }

        [Process]
        public void ProcessVisibleGeometry()
        {
            var camera = this.FrameService.CameraComponent;
            var inView = this.FrameService.CameraComponent.InView;

            for (var i = 0; i < inView.Count; i++)
            {
                var pose = inView[i];
                pose.RenderService.DrawToGBuffer(camera.Camera, pose.Entity);
            }
        }
    }
}
