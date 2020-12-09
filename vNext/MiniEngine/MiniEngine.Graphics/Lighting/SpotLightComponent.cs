﻿using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class SpotLightComponent : AComponent
    {
        public SpotLightComponent(Entity entity, Color color, float strength)
            : base(entity)
        {
            this.Color = color;
            this.Strength = strength;
        }

        public Color Color { get; set; }

        public float Strength { get; set; }
    }
}
