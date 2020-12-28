namespace MiniEngine.Editor.Workspaces
{
    public interface IWorkspace
    {
        public string GetKey() => this.GetType().Name;

        void RenderMainMenuItems() { }

        void RenderWindows() { }

        void Load(EditorState state) { }

        EditorState Save(EditorState state) => state;
    }
}
