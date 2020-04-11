using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class TransparentModel : AModel
    {
        public TransparentModel(Entity entity, Model model)
            : base(entity, model) { }
    }
}