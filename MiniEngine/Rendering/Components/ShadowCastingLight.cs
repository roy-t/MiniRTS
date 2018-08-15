using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Components
{
    public sealed class ShadowCastingLight : PerspectiveCamera
    {
        private const int ShadowMapResolution = 1024;                       

        public ShadowCastingLight(Vector3 position, Vector3 lookAt, Color color)
            : base(new Viewport(0, 0, ShadowMapResolution, ShadowMapResolution))
        {        

            this.ColorVector = color.ToVector3();

            Move(position, lookAt);
        }
       
        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }        
    }
}
