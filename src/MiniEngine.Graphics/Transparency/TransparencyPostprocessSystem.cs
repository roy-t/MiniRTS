using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Transparency
{
    [System]
    public partial class TransparencyPostprocessSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly AverageTransparencyEffect Effect;
        private readonly BlendState AverageParticleBlendState;

        public TransparencyPostprocessSystem(GraphicsDevice device, FrameService frameService, PostProcessTriangle postProcessTriangle, AverageTransparencyEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.PostProcessTriangle = postProcessTriangle;
            this.Effect = effect;
            this.AverageParticleBlendState = CreateAverageParticleBlendState();
        }

        public void OnSet()
        {
            this.Device.BlendState = AverageParticleBlendState;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [Process]
        public void Process()
        {
            this.Effect.Albedo = this.FrameService.TBuffer.Albedo;
            this.Effect.Weights = this.FrameService.TBuffer.Weights;

            this.Effect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }


        public static BlendState CreateAverageParticleBlendState()
        {
            return new BlendState()
            {
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };

        }
    }
}
