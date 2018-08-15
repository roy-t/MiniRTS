using Microsoft.Xna.Framework;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Components
{
    public sealed class Sunlight
    {
        public Sunlight(Color color, Vector3 position, Vector3 lookAt, int cascades)
        {                                    
            this.Color = color;

            this.ShadowCameras = new IViewPoint[cascades];
            this.CascadeSplits = new float[cascades];
            this.CascadeOffsets = new Vector4[cascades];
            this.CascadeScales = new Vector4[cascades];
            this.GlobalShadowMatrix = Matrix.Identity;

            Move(position, lookAt);
        }

        public IViewPoint[] ShadowCameras { get; }
        public float[] CascadeSplits { get; }
        public Vector4[] CascadeOffsets { get; }
        public Vector4[] CascadeScales { get; }
        public Matrix GlobalShadowMatrix { get; set; }

        public Vector3 ColorVector { get; private set; }
        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }                
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }        
        public Vector3 SurfaceToLightVector { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;                        
            this.SurfaceToLightVector = Vector3.Normalize(position - lookAt);
        }       
    }
}
