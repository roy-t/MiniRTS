using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.Systems.Components
{
    public interface IPhysicalComponent : IComponent
    {
        Vector3[] Corners { get; }

        IconType Icon { get; }

        Vector3 Position { get; }
    }
}
