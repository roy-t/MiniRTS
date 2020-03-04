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
            for (var i = 0; i < this.RenderTargets.Count; i++)
            {
                if (this.RenderTargets[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return this.RenderTargets[i].RenderTarget;
                }
            }

            throw new Exception($"Could not find render target with name {name}");
        }

        public IList<RenderTarget2D> GetRenderTargets(IReadOnlyList<string> names)
        {
            var list = new RenderTarget2D[names.Count];
            var insertIndex = 0;
            for (var i = 0; i < this.RenderTargets.Count && insertIndex < names.Count; i++)
            {
                if (names.Contains(this.RenderTargets[i].Name, StringComparer.OrdinalIgnoreCase))
                {
                    list[insertIndex++] = this.RenderTargets[i].RenderTarget;
                }
            }

            if (insertIndex != names.Count)
            {
                throw new Exception($"Could not find all render targets");
            }

            return list;
        }
    }
}
