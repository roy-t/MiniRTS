using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Effects
{
    public sealed class ColorEffect : EffectWrapper
    {
        public ColorEffect()
        {
        }

        public ColorEffect(Effect effect)
        {
            this.Wrap(effect);
        }

        public Vector3 WorldPosition
        {
            set => this.effect.Parameters["WorldPosition"].SetValue(value);
        }

        public Vector3 CameraPosition
        {
            set => this.effect.Parameters["CameraPosition"].SetValue(value);
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }

        public Color VisibleTint
        {
            set => this.effect.Parameters["VisibleTint"].SetValue(value.ToVector4());
        }

        public Color ClippedTint
        {
            set => this.effect.Parameters["ClippedTint"].SetValue(value.ToVector4());
        }

        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector4());
        }        

        public Matrix World
        {
            set => this.effect.Parameters["World"].SetValue(value);
        }

        public Matrix View
        {
            set => this.effect.Parameters["View"].SetValue(value);
        }

        public Matrix Projection
        {
            set => this.effect.Parameters["Projection"].SetValue(value);
        }

        public void Apply(ColorEffectTechniques technique)
        {
            switch (technique)
            {
                case ColorEffectTechniques.Color:
                    this.effect.CurrentTechnique = this.effect.Techniques["ColorEffect"];
                    break;

                case ColorEffectTechniques.ColorGeometryDepthTest:
                    this.effect.CurrentTechnique = this.effect.Techniques["ColorGeometryDepthTestEffect"];
                    break;
                case ColorEffectTechniques.ColorPointDepthTest:
                    this.effect.CurrentTechnique = this.effect.Techniques["ColorPointDepthTestEffect"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(technique), technique, null);
            }

            this.ApplyPass();
        }
    }
}
