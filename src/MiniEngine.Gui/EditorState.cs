using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using Serilog;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class EditorState
    {
        private readonly ILogger Logger;
        private readonly ToolSelector ToolSelector;
        private readonly ToolLinker ToolState;

        public EditorState(ILogger logger, ToolSelector toolSelector, ToolLinker toolState)
        {
            this.Logger = logger;
            this.ToolSelector = toolSelector;
            this.ToolState = toolState;
        }

        bool demo = false;
        float foo;
        float bar;
        Vector3 normal = Vector3.Forward;

        public void Update()
        {
            var p1 = new Property("foo.path");
            var p3 = new Property("bar.normal");
            var p2 = new Property("bar.path");

            if (ImGui.Begin("Window3"))
            {
                foo = this.ToolSelector.Select(foo, p1);
                normal = this.ToolSelector.Select(normal, p3);
                bar = this.ToolSelector.Select(bar, p2);

                if (ImGui.Button("Demo"))
                {
                    demo = !demo;

                }

                if (demo)
                {
                    ImGui.ShowDemoWindow();
                }

                if (ImGui.Button("Reset"))
                {
                    this.ToolState.Reset();
                }

                ImGui.End();
            }
        }
    }
}
