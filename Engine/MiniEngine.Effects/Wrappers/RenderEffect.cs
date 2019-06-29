using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class RenderEffect : EffectWrapper
    {
        public RenderEffect()
        {
        }

        public RenderEffect(Effect renderEffect)
        {
            this.Wrap(renderEffect);
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

        public void Apply(RenderEffectTechniques technique)
        {
            switch (technique)
            {
                case RenderEffectTechniques.ShadowMap:
                    this.effect.CurrentTechnique = this.effect.Techniques["ShadowMap"];
                    break;
                case RenderEffectTechniques.GrayScale:
                    this.effect.CurrentTechnique = this.effect.Techniques["GrayScale"];
                    break;
                case RenderEffectTechniques.Textured:
                    this.effect.CurrentTechnique = this.effect.Techniques["Textured"];
                    break;
                case RenderEffectTechniques.Deferred:
                    this.effect.CurrentTechnique = this.effect.Techniques["Deferred"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(technique), technique, null);
            }

            this.ApplyPass();
        }
    }
}