using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class CombineEffect : EffectWrapper
    {
        public CombineEffect(Effect effect) : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["CombineTechnique"];
        }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }

        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public Texture2D Depth { set => this.Effect.Parameters["Depth"].SetValue(value); }
    }
}
