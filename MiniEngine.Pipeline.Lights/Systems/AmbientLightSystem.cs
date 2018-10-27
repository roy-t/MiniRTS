using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class AmbientLightSystem : ISystem
    {
        private readonly Dictionary<Entity, Color> Lights;

        public AmbientLightSystem()
        {
            this.Lights = new Dictionary<Entity, Color>();
        }

        public bool Contains(Entity entity) => this.Lights.ContainsKey(entity);


        public string Describe(Entity entity)
        {
            var color = this.Lights[entity];
            return $"ambient light, color {color}";
        }

        public void Remove(Entity entity) => this.Lights.Remove(entity);

        public void Add(Entity entity, Color color) => this.Lights.Add(entity, color);

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