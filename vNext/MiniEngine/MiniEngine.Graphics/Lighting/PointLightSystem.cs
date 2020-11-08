using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public sealed class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PointLightEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle; // TODO: replace with sphere or other geom that better fits the influence of the light source

        public PointLightSystem(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, PointLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.FullScreenTriangle = fullScreenTriangle;

            this.Effect = effect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
            this.Device.SamplerStates[3] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        public void Process(PointLightComponent pointLight)
        {
            this.Effect.CameraPosition = this.FrameService.Camera.Position;
            this.Effect.Diffuse = this.FrameService.GBuffer.Diffuse;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.Camera.ViewProjection);

            this.Effect.Position = pointLight.Position;
            this.Effect.Color = pointLight.Color;
            this.Effect.Strength = pointLight.Strength;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
