using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public partial class GeometrySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly GeometryRenderService RenderService;
        private readonly FrameService FrameService;

        public GeometrySystem(GraphicsDevice device, GeometryRenderService renderService, FrameService frameService)
        {
            this.Device = device;
            this.RenderService = renderService;
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
                this.RenderService.DrawToGBuffer(camera.Camera, pose.Entity);
            }
        }
    }
}
