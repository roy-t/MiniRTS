using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Components
{
    public sealed class Projector : IComponent
    {        
        public Projector(Texture2D texture, Color tint, Vector3 position, Vector3 lookAt, Meters minDistance, Meters maxDistance)
        {            
            this.Texture = texture;            
            this.Tint = tint;
            this.MinDistance = minDistance;
            this.MaxDistance = maxDistance;
            this.ViewPoint = new PerspectiveCamera(1);

            this.ViewPoint.Move(position, lookAt);
            this.ViewPoint.SetPlanes(minDistance, maxDistance);
        }
        
        public Texture2D Texture { get; }             
        public Color Tint { get; private set; }
        public float MaxDistance { get; private set; }
        public float MinDistance { get; private set; }
        public PerspectiveCamera ViewPoint { get; }


        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Projector");
            description.AddLabel("Texture", this.Texture);
            description.AddProperty("Tint", this.Tint, t => this.Tint = t, MinMaxDescription.ZeroToOne);
            description.AddProperty("Min distance", this.MinDistance, m => this.SetMinDistance(m), MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Max distance", this.MaxDistance, m => this.SetMaxDistance(m), MinMaxDescription.ZeroToInfinity);            
            description.AddProperty("Position", this.ViewPoint.Position, p => this.ViewPoint.Move(p, this.ViewPoint.LookAt), MinMaxDescription.MinusInfinityToInfinity);
            description.AddProperty("Look at", this.ViewPoint.LookAt, l => this.ViewPoint.Move(this.ViewPoint.Position, l), MinMaxDescription.MinusInfinityToInfinity);            
            description.AddProperty("Field of view", this.ViewPoint.FieldOfView, f => this.ViewPoint.SetFieldOfView(f), new MinMaxDescription(0, MathHelper.Pi));

            return description;
        }

        private void SetMaxDistance(float m)
        {
            this.MaxDistance = m;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }

        private void SetMinDistance(float m)
        {
            this.MinDistance = m;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }
    }
}
