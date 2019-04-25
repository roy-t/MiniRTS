using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.Pipeline.Models.Components
{
    [Label(nameof(OpaqueModel))]
    public sealed class OpaqueModel : AModel
    {
        public OpaqueModel(Model model, Pose pose)
            : base(model, pose) { }       
    }
}