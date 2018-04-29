using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Lighting.Components
{
    public sealed class Sunlight
    {
        public Sunlight(Color color, Vector3 position, Vector3 lookAt)
        {                                    
            this.Color = color;
            Move(position, lookAt);
        }

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
