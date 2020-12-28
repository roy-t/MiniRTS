using ImGuiNET;
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

        public void RenderWindows() => this.EntityEditor.Draw();



        public void Load(EditorState state) => this.EntityEditor.SelectedEntity = state.Entity;

        public EditorState Save(EditorState state) => state with { Entity = this.EntityEditor.SelectedEntity };

    }
}
