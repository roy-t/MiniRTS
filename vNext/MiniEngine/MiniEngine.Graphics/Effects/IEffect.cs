using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Effects
{
    public interface IEffect
    {
        void Apply();
    }

    public interface I3DEffect : IEffect
    {
        Matrix WorldViewProjection { set; }
    }
}
