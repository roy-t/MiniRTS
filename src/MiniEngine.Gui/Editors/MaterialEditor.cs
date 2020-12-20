using System;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class MaterialEditor : AEditor<Material>
    {
        private readonly Texture2DEditor Texture2DEditor;

        public MaterialEditor(Texture2DEditor texture2DEditor)
        {
            this.Texture2DEditor = texture2DEditor;
        }

        public override bool Draw(string name, Func<Material> get, Action<Material> set)
        {
            var material = get();

            var changed = false;
            changed |= this.Texture2DEditor.Draw($"{name}.{nameof(Material.Albedo)}", () => material.Albedo, t => material.Albedo = t);
            changed |= this.Texture2DEditor.Draw($"{name}.{nameof(Material.Normal)}", () => material.Normal, t => material.Normal = t);
            changed |= this.Texture2DEditor.Draw($"{name}.{nameof(Material.Metalicness)}", () => material.Metalicness, t => material.Metalicness = t);
            changed |= this.Texture2DEditor.Draw($"{name}.{nameof(Material.Roughness)}", () => material.Roughness, t => material.Roughness = t);
            changed |= this.Texture2DEditor.Draw($"{name}.{nameof(Material.AmbientOcclusion)}", () => material.AmbientOcclusion, t => material.AmbientOcclusion = t);

            return changed;
        }
    }
}
