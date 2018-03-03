using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class ShadowCastingLight : Camera
    {
        private const int ShadowMapResolution = 1024;                       

        public ShadowCastingLight(GraphicsDevice device, Vector3 position, Vector3 lookAt, Color color, float intensity)
            : base(new Viewport(0, 0, ShadowMapResolution, ShadowMapResolution))
        {        
            this.ShadowMap = new RenderTarget2D(device, ShadowMapResolution, ShadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
                        
            this.ColorVector = color.ToVector3();
            this.Intensity = intensity;

            Move(position, lookAt);
        }

        public RenderTarget2D ShadowMap { get; }
       
        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }

        public float Intensity { get; }        
    }
}
