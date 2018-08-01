using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Systems
{
    public sealed class AmbientLightSystem
    {
        private readonly Dictionary<Entity, Color> Lights;

        public AmbientLightSystem()
        {
            this.Lights = new Dictionary<Entity, Color>();
        }

        public void Add(Entity entity, Color color)
        {
            this.Lights.Add(entity, color);
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
        }

        public Color ComputeAmbientLightZeroAlpha()
        {
            var accumulate = Color.TransparentBlack;
            foreach (var color in this.Lights.Values)
            {
                accumulate.R += color.R;
                accumulate.G += color.G;
                accumulate.B += color.B;
            }

            return accumulate;
        }
    }
}
