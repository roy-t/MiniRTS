using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class Sunlight
    {
        private const int ShadowMapResolution = 1024;       

        public Sunlight(GraphicsDevice device)
        {         
            Move(Vector3.Backward * 10, Vector3.Zero);
        }

        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }                
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }        

        public void Move(Vector3 position, Vector3 lookAt)
        {
            
        }        
    }
}
