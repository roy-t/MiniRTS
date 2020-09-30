using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.Effects
{
    [Service]
    public sealed class EffectFactory
    {
        private const string EffectsFolder = "Effects";

        private readonly ContentManager Content;

        public EffectFactory(ContentManager content)
        {
            this.Content = content;
        }

        public T Construct<T>()
            where T : EffectWrapper
        {
            var type = typeof(T);
            var constructor = type.GetConstructor(new Type[] { typeof(Effect) });
            if (constructor == null)
            {
                throw new Exception($"Could not construct {type.FullName} as it does not have a constructor with a single Microsoft.Xna.Framework.Graphics.Effect argument");
            }

            var effect = this.Content.Load<Effect>(Path.Combine(EffectsFolder, typeof(T).Name));
            return (T)constructor.Invoke(new[] { effect });
        }
    }
}
