using System;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class GeometryDataEditor : AEditor<GeometryData>
    {
        private readonly BoundingSphereEditor BoundingSphereEditor;

        public GeometryDataEditor(BoundingSphereEditor boundingSphereEditor)
        {
            this.BoundingSphereEditor = boundingSphereEditor;
        }

        public override bool Draw(string name, Func<GeometryData> get, Action<GeometryData> set)
        {
            var geometry = get();

            ImGui.Text($"Name: {geometry.Name}");
            ImGui.Text($"Primitives: {geometry.Primitives}");
            return this.BoundingSphereEditor.Draw($"{name}.Bounds", () => geometry.Bounds, b => { });
        }
    }
}
