using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public partial class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PointLightEffect Effect;
        private readonly PostProcessTriangle PostProcessTriangle; // TODO: replace with sphere or other geom that better fits the influence of the light source

        public PointLightSystem(GraphicsDevice device, PostProcessTriangle postProcessTriangle, PointLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.PostProcessTriangle = postProcessTriangle;
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

        [ProcessAll]
        public void Process(PointLightComponent pointLight, TransformComponent transform)
        {
            this.Effect.CameraPosition = this.FrameService.CamereComponent.Camera.Position;
            this.Effect.Diffuse = this.FrameService.GBuffer.Diffuse;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CamereComponent.Camera.ViewProjection);

            this.Effect.Position = transform.Matrix.Translation;
            this.Effect.Color = pointLight.Color;
            this.Effect.Strength = pointLight.Strength;

            this.Effect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
