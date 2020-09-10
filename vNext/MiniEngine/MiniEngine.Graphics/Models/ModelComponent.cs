using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Models
{
    public sealed class ModelComponent : AComponent
    {
        public ModelComponent(Entity entity, Model model)
            : base(entity)
        {
            this.Model = model;
        }

        public Model Model { get; }
    }
}
