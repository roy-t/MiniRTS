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

        public int Resolution { get => this.DepthMapArray.Width; set => this.ChangeResolution(value); }

        public RenderTarget2D DepthMapArray { get; private set; }

        public float[] Cascades { get; }

        public Matrix GlobalShadowMatrix { get; set; }

        public float[] Splits { get; }

        public Vector4[] Offsets { get; }

        public Vector4[] Scales { get; }

        public void Dispose()
            => this.DepthMapArray.Dispose();

        public static CascadedShadowMapComponent Create(Entity entity, GraphicsDevice device, int resolution, float[] cascades)
        {
            var depthMapArray = new RenderTarget2D(device, resolution, resolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents, false, cascades.Length);
            return new CascadedShadowMapComponent(entity, depthMapArray, cascades);
        }

        private void ChangeResolution(int value)
        {
            value = Math.Clamp(value, 128, 4096);
            var depthMapArray = new RenderTarget2D(this.DepthMapArray.GraphicsDevice, value, value, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents, false, this.Cascades.Length);

            this.DepthMapArray.Dispose();
            this.DepthMapArray = depthMapArray;
        }
    }
}
