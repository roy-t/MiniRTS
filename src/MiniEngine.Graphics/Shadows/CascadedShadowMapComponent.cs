using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class CascadedShadowMapComponent : AComponent, IDisposable
    {
        public CascadedShadowMapComponent(Entity entity, RenderTarget2D depthMapArray, float[] cascades)
            : base(entity)
        {
            if (cascades.Length != SunlightEffect.CascadeCount)
            {
                throw new ArgumentException($"{cascades}.Length ({cascades.Length})!= {nameof(SunlightEffect)}.Length{nameof(SunlightEffect.CascadeCount)}");
            }

            this.DepthMapArray = depthMapArray;
            this.Cascades = cascades;

            this.GlobalShadowMatrix = Matrix.Identity;
            this.Splits = new float[cascades.Length];
            this.Offsets = new Vector4[cascades.Length];
            this.Scales = new Vector4[cascades.Length];

        }

        public RenderTarget2D DepthMapArray { get; }
        public float[] Cascades { get; }

        public float Resolution => this.DepthMapArray.Width;

        public static CascadedShadowMapComponent Create(Entity entity, GraphicsDevice device, int resolution, float[] cascades)
        {
            var depthMapArray = new RenderTarget2D(device, resolution, resolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents, false, cascades.Length);
            return new CascadedShadowMapComponent(entity, depthMapArray, cascades);
        }

        public Matrix GlobalShadowMatrix { get; set; }

        public float[] Splits { get; }
        public Vector4[] Offsets { get; }
        public Vector4[] Scales { get; }

        public void Dispose()
            => this.DepthMapArray.Dispose();
    }
}
