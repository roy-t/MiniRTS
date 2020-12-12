using System;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class SpotLightComponent : AComponent
    {
        private const float MinimumLightInfluence = 1.0f / 256.0f;

        public SpotLightComponent(Entity entity, Color color, float strength)
            : base(entity)
        {
            this.Color = color;
            this.Strength = strength;
        }

        public Color Color { get; set; }

        public float Strength { get; set; }

        public float LightCutOff()
            => MathF.Sqrt(this.Strength / MinimumLightInfluence);

        public float Attenuation(float distance)
            => this.Strength / (distance * distance);
    }
}
