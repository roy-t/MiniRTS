using System;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    public sealed class DoubleBufferedRenderTarget : IDisposable
    {
        private readonly RenderTarget2D A;
        private readonly RenderTarget2D B;

        private bool writeToA;

        public DoubleBufferedRenderTarget(GraphicsDevice device, int dimensions, SurfaceFormat format)
        {
            this.A = new RenderTarget2D(device, dimensions, dimensions, false, format, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this.B = new RenderTarget2D(device, dimensions, dimensions, false, format, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            this.writeToA = true;
        }

        public RenderTarget2D WriteTarget => this.writeToA ? this.A : this.B;

        public RenderTarget2D ReadTarget => this.writeToA ? this.B : this.A;

        public void Swap() => this.writeToA = !this.writeToA;

        public void Dispose()
        {
            this.A.Dispose();
            this.B.Dispose();
        }
    }
}
