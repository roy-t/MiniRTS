using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class ShadowMap : IComponent, IDisposable
    {
        public ShadowMap(Entity entity, RenderTarget2D depthMap, RenderTarget2D colorMap, int index, IViewPoint viewPoint)
        {
            this.Entity = entity;
            this.DepthMap = depthMap;
            this.ColorMap = colorMap;
            this.Index = index;
            this.ViewPoint = viewPoint;
        }

        public Entity Entity { get; }

        [Editor(nameof(Width))]
        public int Width => this.DepthMap.Width;

        [Editor(nameof(Height))]
        public int Height => this.DepthMap.Height;

        [Editor(nameof(DepthMap), null, nameof(Index))]
        public RenderTarget2D DepthMap { get; }

        [Editor(nameof(ColorMap), null, nameof(Index))]
        public RenderTarget2D ColorMap { get; }

        [Editor(nameof(Index))]
        public int Index { get; }

        public IViewPoint ViewPoint { get; }

        public void Dispose()
        {
            this.DepthMap.Dispose();
            this.ColorMap.Dispose();
        }
    }
}