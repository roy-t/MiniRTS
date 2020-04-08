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

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }

        public Vector3 CameraPosition
        {
            set => this.effect.Parameters["CameraPosition"].SetValue(value);
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

        public Texture2D ReflectionMap
        {
            set => this.effect.Parameters["ReflectionMap"].SetValue(value);
        }

        public Texture2D Mask
        {
            set => this.effect.Parameters["Mask"].SetValue(value);
        }

        public TextureCube Skybox
        {
            set => this.effect.Parameters["Skybox"].SetValue(value);
        }

        public Matrix[] BoneTransforms
        {
            set => this.effect.Parameters["BoneTransforms"].SetValue(value);
        }

        public Vector2 TextureScale
        {
            set => this.effect.Parameters["TextureScale"].SetValue(value);
        }

        public Vector2 TextureOffset
        {
            set => this.effect.Parameters["TextureOffset"].SetValue(value);
        }

        public void Apply(RenderEffectTechniques technique)
        {
            this.effect.CurrentTechnique = technique switch
            {
                RenderEffectTechniques.ShadowMap => this.effect.Techniques["ShadowMap"],
                RenderEffectTechniques.ShadowMapSkinned => this.effect.Techniques["ShadowMapSkinned"],
                RenderEffectTechniques.Textured => this.effect.Techniques["Textured"],
                RenderEffectTechniques.Deferred => this.effect.Techniques["Deferred"],
                RenderEffectTechniques.DeferredSkinned => this.effect.Techniques["DeferredSkinned"],
                _ => throw new ArgumentOutOfRangeException(nameof(technique), technique, null),
            };
            this.ApplyPass();
        }

        public static bool TechniqueSupportsSkinning(RenderEffectTechniques technique)
        {
            switch (technique)
            {
                case RenderEffectTechniques.ShadowMap:
                case RenderEffectTechniques.ShadowMapSkinned:
                case RenderEffectTechniques.Deferred:
                case RenderEffectTechniques.DeferredSkinned:
                    return true;
            }

            return false;
        }

        public static RenderEffectTechniques GetSkinnedTechnique(RenderEffectTechniques technique)
        {
            return technique switch
            {
                RenderEffectTechniques.ShadowMap => RenderEffectTechniques.ShadowMapSkinned,
                RenderEffectTechniques.Deferred => RenderEffectTechniques.DeferredSkinned,

                _ => technique,
            };
        }
    }
}