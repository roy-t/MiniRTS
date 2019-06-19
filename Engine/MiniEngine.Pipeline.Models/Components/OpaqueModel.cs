using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class OpaqueModel : AModel
    {
        public OpaqueModel(Model model, Pose pose)
            : base(model, pose) { }       
    }
}