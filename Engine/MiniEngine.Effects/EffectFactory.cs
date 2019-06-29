using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class EffectFactory
    {
        private static readonly string Folder = "Effects";
        private readonly ContentManager Content;

        public EffectFactory(ContentManager content)
        {
            this.Content = content;
        }

        public T Construct<T>()
            where T : EffectWrapper, new()
        {
            var wrapper = new T();
            wrapper.Wrap(this.Content.Load<Effect>(Path.Combine(Folder, typeof(T).Name)));

            return wrapper;
        }
    }
}
