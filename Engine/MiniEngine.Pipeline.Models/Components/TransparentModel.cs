using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class TransparentModel : AModel
    {
        public TransparentModel(Entity entity, Model model, Pose pose)
            : base(entity, model, pose) { }
    }
}