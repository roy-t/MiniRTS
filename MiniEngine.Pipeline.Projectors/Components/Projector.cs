using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Projectors.Components
{
    public sealed class Projector : IComponent
    {
        public Projector(Texture2D texture, Vector3 position, Vector3 lookAt)
        {
            this.Texture = texture;
            this.Position = position;
            this.LookAt = lookAt;
        }

        public Texture2D Texture { get; }     
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }


        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Projector");
            description.AddLabel("Texture", this.Texture);
            description.AddProperty("Position", this.Position, p => this.Position = p, MinMaxDescription.MinusInfinityToInfinity);
            description.AddProperty("Look at", this.LookAt, l => this.LookAt = l, MinMaxDescription.MinusInfinityToInfinity);

            return description;
        }
    }
}
