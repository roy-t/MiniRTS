using MiniEngine.Configuration;
using MiniEngine.Editor.Workspaces.Editors;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class EntityWorkspace : IWorkspace
    {
        private readonly EntityEditor EntityEditor;

        public EntityWorkspace(EntityEditor entityEditor)
        {
            this.EntityEditor = entityEditor;
        }

        public void RenderWindows()
            => this.EntityEditor.Draw();
    }
}
