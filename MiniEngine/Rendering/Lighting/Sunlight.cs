using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class Sunlight
    {
        public const int Resolution = 1024;
        public const int Cascades = 4;
       
        public Sunlight(GraphicsDevice device)
        {
            this.ShadowMap = new RenderTarget2D(
                device,
                Resolution,
                Resolution,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents,
                false,
                Cascades);

            this.CascadeSplits = new[]
            {
                0.05f,
                0.15f,
                0.5f,
                1.0f
            };


            Move(Vector3.Backward * 10, Vector3.Zero);
        }

        public RenderTarget2D ShadowMap { get; }
        public float[] CascadeSplits { get; }


        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }                
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }        
        public Vector3 Direction { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;
            this.Direction = Vector3.Normalize(lookAt - position);
        }        
    }
}
