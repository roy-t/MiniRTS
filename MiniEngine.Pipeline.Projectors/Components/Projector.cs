using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Components
{
    public sealed class Projector : IComponent
    {
        private const float Epsilon = 0.001f;

        public Projector(Texture2D texture, Color tint, Vector3 position, Vector3 lookAt, Meters minDistance, Meters maxDistance)
        {            
            this.Texture = texture;            
            this.Tint = tint;

            this.ViewPoint = new PerspectiveCamera(1);
            this.ViewPoint.Move(position, lookAt);

            this.SetMinDistance(minDistance);
            this.SetMaxDistance(maxDistance);

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

        public void SetMinDistance(float distance)
        {
            distance = MathHelper.Clamp(distance, Epsilon, this.MaxDistance - Epsilon);

            this.MinDistance = distance;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }

        public void SetMaxDistance(float distance)
        {
            distance = MathHelper.Clamp(distance, this.MinDistance + Epsilon, float.MaxValue);

            this.MaxDistance = distance;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }        
    }
}
