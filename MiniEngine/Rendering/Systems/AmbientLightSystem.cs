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
            var accumlate = Color.TransparentBlack;
            foreach (var color in this.Lights.Values)
            {
                accumlate.R += color.R;
                accumlate.G += color.G;
                accumlate.B += color.B;
            }

            return accumlate;
        }
    }
}
