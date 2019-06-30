using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;

namespace MiniEngine.UI.Utilities
{
    public sealed class RenderTargetDescriber
    {        
        public RenderTargetDescriber(DeferredRenderPipeline renderPipeline)
        {
            var gBuffer = renderPipeline.GetGBuffer();

            this.RenderTargets = new List<RenderTargetDescription>()
            {
                new RenderTargetDescription(gBuffer.DiffuseTarget, "diffuse", 0),
                new RenderTargetDescription(gBuffer.NormalTarget, "normal", 1),
                new RenderTargetDescription(gBuffer.ParticleTarget, "particle", 2),
                new RenderTargetDescription(gBuffer.DepthTarget, "depth", 3),
                new RenderTargetDescription(gBuffer.LightTarget, "light", 4),
                new RenderTargetDescription(gBuffer.BlurTarget, "blur", 5),
                new RenderTargetDescription(gBuffer.CombineTarget, "combine", 6),
                new RenderTargetDescription(gBuffer.FinalTarget, "final", 7),
            };
        }

        public IReadOnlyList<RenderTargetDescription> RenderTargets { get; }

        public RenderTarget2D GetRenderTarget(string name)
        {
            return this.RenderTargets
                .Where(x => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.RenderTarget)
                .FirstOrDefault();
        }

        public IList<RenderTarget2D> GetRenderTargets(IEnumerable<string> names)
        {
            return this.RenderTargets
                .OrderBy(x => x.Order)
                .Where(x => names.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                .Select(x => x.RenderTarget)
                .ToList();
        }
    }
}
