using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class PropertyInspector
    {
        public PropertyInspector()
        {
            this.Textures = new ObservableCollection<Texture2D>();
        }

        public ObservableCollection<Texture2D> Textures { get; }
    }
}
