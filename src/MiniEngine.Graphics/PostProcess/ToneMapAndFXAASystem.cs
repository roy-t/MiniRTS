﻿using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.PostProcess
{
    [System]
    public partial class ToneMapSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly ToneMapAndFXAAEffect Effect;
        private readonly PostProcessTriangle PostProcessTriangle;

        public ToneMapSystem(GraphicsDevice device, PostProcessTriangle postProcessTriangle, ToneMapAndFXAAEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effect;
            this.PostProcessTriangle = postProcessTriangle;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTarget(this.FrameService.PBuffer.ToneMap);
        }

        [Process]
        public void Process()
        {
            this.Effect.Color = this.FrameService.LBuffer.Light;
            this.Effect.ColorSampler = SamplerState.LinearClamp;

            this.Effect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
