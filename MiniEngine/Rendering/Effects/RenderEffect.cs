using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Effects
{
    public sealed class RenderEffect : EffectWrapper
    {
        public RenderEffect()
        {

        }

        public RenderEffect(Effect renderEffect)
        {
            Wrap(renderEffect);
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

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["Texture"].SetValue(value);
        }

        public Texture2D SpecularMap
        {
            set => this.effect.Parameters["SpecularMap"].SetValue(value);
        }

        public Texture2D NormalMap
        {
            set => this.effect.Parameters["NormalMap"].SetValue(value);
        }

        public Texture2D Mask
        {
            set => this.effect.Parameters["Mask"].SetValue(value);
        }

        public void Apply(Techniques technique)
        {            
            switch (technique)
            {
                case Techniques.MRT:
                    this.effect.CurrentTechnique = this.effect.Techniques["MRT"];
                    break;
                case Techniques.ShadowMap:
                    this.effect.CurrentTechnique = this.effect.Techniques["ShadowMap"];
                    break;
                case Techniques.ColorMap:
                    this.effect.CurrentTechnique = this.effect.Techniques["ColorMap"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(technique), technique, null);
            }

            ApplyPass();
        }        
    }
}
