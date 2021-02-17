using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Particles;
using MiniEngine.Gui.Tools;

namespace MiniEngine.Editor.Windows.Particles
{
    [Service]
    public sealed class EmitterTool : ATool<ParticleEmitter>
    {
        private readonly ParticleWindow EditorWindow;

        public override string Name => "Emitter";

        public EmitterTool(ParticleWindow editorWindow)
        {
            this.EditorWindow = editorWindow;
        }

        public override bool HeaderValue(ref ParticleEmitter value, ToolState tool)
        {
            if (ImGui.SmallButton("Edit"))
            {
                this.EditorWindow.SetEmitter(value);
            }

            return false;
        }
    }
}
