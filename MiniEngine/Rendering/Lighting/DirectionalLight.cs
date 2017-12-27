using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Lighting
{
    public struct DirectionalLight
    {        
        public DirectionalLight(Vector3 direction, Color color)
        {
            this.Direction = direction;
            this.ColorVector = color.ToVector3();
        }

        public Vector3 Direction { get; set; }
        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }        
    }
}
