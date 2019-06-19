using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Effects
{
    public sealed class TextureEffect : EffectWrapper
    {
        public TextureEffect()
        {            
        }

        public TextureEffect(Effect effect)
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

        public Texture2D Texture
        {
            set => this.effect.Parameters["Texture"].SetValue(value);
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

        public void Apply(TextureEffectTechniques technique)
        {
            switch (technique)
            {
                case TextureEffectTechniques.Texture:
                    this.effect.CurrentTechnique = this.effect.Techniques["TextureEffect"];
                    break;

                case TextureEffectTechniques.TextureGeometryDepthTest:
                    this.effect.CurrentTechnique = this.effect.Techniques["TextureGeometryDepthTestEffect"];
                    break;
                case TextureEffectTechniques.TexturePointDepthTest:
                    this.effect.CurrentTechnique = this.effect.Techniques["TexturePointDepthTestEffect"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(technique), technique, null);
            }

            this.ApplyPass();
        }
    }
}
