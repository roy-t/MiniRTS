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
                this.ToolSelector.BeginTable("window");

                this.ToolSelector.Select(ref this.foo, p1);
                this.ToolSelector.Select(ref this.normal, p3);
                this.ToolSelector.Select(ref this.bar, p2);

                this.ToolSelector.EndTable();
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
