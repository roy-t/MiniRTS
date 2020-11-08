﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class ImageBasedLightEffect : EffectWrapper
    {
        public ImageBasedLightEffect(EffectFactory factory) : base(factory.Load<ImageBasedLightEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ImageBasedLightTechnique"];
        }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }

        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public Texture2D Depth { set => this.Effect.Parameters["Depth"].SetValue(value); }

        public Texture2D Material { set => this.Effect.Parameters["Material"].SetValue(value); }

        public TextureCube Irradiance { set => this.Effect.Parameters["Irradiance"].SetValue(value); }

        public TextureCube Environment { set => this.Effect.Parameters["Environment"].SetValue(value); }

        public Texture2D BrdfLut { set => this.Effect.Parameters["BrdfLut"].SetValue(value); }

        public Matrix InverseViewProjection { set => this.Effect.Parameters["InverseViewProjection"].SetValue(value); }

        public Vector3 CameraPosition { set => this.Effect.Parameters["CameraPosition"].SetValue(value); }

        public int MaxReflectionLod { set => this.Effect.Parameters["MaxReflectionLod"].SetValue(value); }
    }
}
