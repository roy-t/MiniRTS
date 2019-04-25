using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.Pipeline.Models.Components
{
    [Label(nameof(TransparentModel))]
    public sealed class TransparentModel : AModel
    {
        public TransparentModel(Model model, Pose pose)
            : base(model, pose) { }        
    }
}