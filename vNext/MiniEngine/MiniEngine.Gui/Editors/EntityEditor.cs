using System;
using MiniEngine.Systems;

namespace MiniEngine.Gui.Editors
{
    public sealed class EntityEditor : AEditor<Entity>
    {
        public override void Draw(string name, Func<Entity> get, Action<Entity> set)
        {
            // no-op
        }
    }
}
