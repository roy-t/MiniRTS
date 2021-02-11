namespace MiniEngine.Gui.Windows
{
    public interface IWindow
    {
        string Name { get; }

        bool AllowTransparency { get; }

        void RenderContents();
    }
}
