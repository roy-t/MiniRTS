using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class Sunlight
    {
        public const int Resolution = 2048;
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

            this.ShadowCameras = new ViewPoint[4];
            this.CascadeSplitsUV = new float[4];
            this.CascadeOffsets  = new Vector4[4];
            this.CascadeScales   = new Vector4[4];
            this.GlobalShadowMatrix = Matrix.Identity;            

            this.Color = Color.White;

            Move(Vector3.Backward * 10, Vector3.Zero);
        }

        public RenderTarget2D ShadowMap { get; }
        public float[] CascadeSplits { get; }

        // Variables set when calculating the shadows
        public ViewPoint[] ShadowCameras { get; }
        public float[] CascadeSplitsUV { get; }    
        public Vector4[] CascadeOffsets { get; }
        public Vector4[] CascadeScales { get; }
        public Matrix  GlobalShadowMatrix { get; set; }

        public Vector3 ColorVector { get; private set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }                
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }        
        public Vector3 LightToSurfaceDirection { get; private set; }
        public Vector3 SurfaceToLightVector { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;            
            this.LightToSurfaceDirection = Vector3.Normalize(lookAt - position);
            this.SurfaceToLightVector = -this.LightToSurfaceDirection;
        }       
    }
}
