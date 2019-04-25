using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Components
{
    [Label(nameof(Outline))]
    public sealed class Outline : IComponent
    {        
        public Outline(AModel model, Color color3D, Color color2d)
        {
            this.Model = model;
            this.Color3D = color3D;
            this.Color2D = color2d;
        }

        public AModel Model { get; }

        [Editor(nameof(Color3D), nameof(Color3D))]
        public Color Color3D { get; set; }

        [Editor(nameof(Color2D), nameof(Color2D))]
        public Color Color2D { get; set; }        
    }
}
