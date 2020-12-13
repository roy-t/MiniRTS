using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class ProjectorEffect : EffectWrapper
    {
        public ProjectorEffect()
        {
        }

        public ProjectorEffect(Effect effect)
        {
            this.Wrap(effect);
        }

        public float MaxDistance
        {
            set => this.effect.Parameters["MaxDistance"].SetValue(value);
        }

        public Color Tint
        {
            set => this.effect.Parameters["Tint"].SetValue(value.ToVector4());
        }

        public Vector3 ProjectorPosition
        {
            set => this.effect.Parameters["ProjectorPosition"].SetValue(value);
        }

        public Vector3 ProjectorForward
        {
            set => this.effect.Parameters["ProjectorForward"].SetValue(value);
        }

        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }       

        public Texture2D ProjectorMap
        {
            set => this.effect.Parameters["ProjectorMap"].SetValue(value);
        }

        public Texture2D Mask
        {
            set => this.effect.Parameters["Mask"].SetValue(value);
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

        public Matrix ProjectorViewProjection
        {
            set => this.effect.Parameters["ProjectorViewProjection"].SetValue(value);
        }

        public void Apply(ProjectorEffectTechniques technique)
        {
            switch (technique)
            {
                case ProjectorEffectTechniques.Projector:
                    this.effect.CurrentTechnique = this.effect.Techniques["ProjectorEffect"];
                    break;
                case ProjectorEffectTechniques.ProjectorOverdraw:
                    this.effect.CurrentTechnique = this.effect.Techniques["ProjectorOverdrawEffect"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(technique), technique, null);
            }

            this.ApplyPass();
        }
    }
}
