using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class ShadowMapComponent : AComponent, IDisposable
    {
        public ShadowMapComponent(Entity entity, RenderTarget2D depthMap)
            : base(entity)
        {
            this.DepthMap = depthMap;
        }

        public RenderTarget2D DepthMap { get; }

        public static ShadowMapComponent Create(Entity entity, GraphicsDevice device, int resolution)
        {
            var renderTarget = new RenderTarget2D(device, resolution, resolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            return new ShadowMapComponent(entity, renderTarget);
        }

        public void Dispose()
            => this.DepthMap.Dispose();
    }
}
