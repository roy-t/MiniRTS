using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.UI
{
    public sealed class UIRenderer
    {
        public UIRenderer(DeferredRenderPipeline renderPipeline)
        {
            var gBuffer = renderPipeline.GetGBuffer();

            this.RenderTargets = new List<RenderTargetDescription>()
            {
                new RenderTargetDescription(gBuffer.DepthTarget, "diffuse", 0),
                new RenderTargetDescription(gBuffer.NormalTarget, "normal", 1),
                new RenderTargetDescription(gBuffer.ParticleTarget, "particle", 2),
                new RenderTargetDescription(gBuffer.DepthTarget, "depth", 3),
                new RenderTargetDescription(gBuffer.LightTarget, "light", 4),
                new RenderTargetDescription(gBuffer.BlurTarget, "blur", 5),
                new RenderTargetDescription(gBuffer.CombineTarget, "combine", 6),
                new RenderTargetDescription(gBuffer.FinalTarget, "final", 7),
            };
        }

        public IReadOnlyList<RenderTargetDescription> RenderTargets;

        public RenderTarget2D GetRenderTarget(string name)
        {
            var target = this.RenderTargets.FirstOrDefault(description => description.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return target?.RenderTarget;
        }
    }
}
