using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Content
{
    public sealed class TexturePatcher
    {
        private readonly ContentManager Content;
        private readonly Texture2D DiffuseFallback;
        private readonly Texture2D SpecularFallback;
        private readonly Texture2D NormalFallback;

        public TexturePatcher(GraphicsDevice device, ContentManager content)
        {
            this.Content = content;
            this.DiffuseFallback = new Texture2D(device, 1, 1);
            this.DiffuseFallback.SetData(
                new[]
                {
                    Color.Magenta
                });

            this.SpecularFallback = this.Content.Load<Texture2D>("null_specular");
            this.NormalFallback= this.Content.Load<Texture2D>("null_normal");
        }

        public void Patch(Model model)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    Swap(effect, "Texture", this.DiffuseFallback);
                    Swap(effect, "SpecularMap", this.SpecularFallback);
                    Swap(effect, "NormalMap", this.NormalFallback);
                }
            }
        }

        private void Swap(Effect effect, string parameter, Texture2D fallback)
        {
            var texture = effect.Parameters[parameter].GetValueTexture2D();
            if (texture == null)
            {
                effect.Parameters[parameter].SetValue(fallback);
            }
            else if(texture.Name.EndsWith("_0"))
            {
                if (parameter == "SpecularMap")
                {
                    
                }

                var replacement = this.Content.Load<Texture2D>(texture.Name.Substring(0, texture.Name.Length - 2));
                effect.Parameters[parameter].SetValue(replacement);
            }
        }       
    }
}
