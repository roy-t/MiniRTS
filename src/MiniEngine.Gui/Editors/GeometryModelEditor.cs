using System;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using System.Linq;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class GeometryModelEditor : AEditor<GeometryModel>
    {
        private readonly BoundingSphereEditor BoundingSphereEditor;
        private readonly GeometryMeshEditor GeometryMeshEditor;

        public GeometryModelEditor(BoundingSphereEditor boundingSphereEditor, GeometryMeshEditor geometryMeshEditor)
        {
            this.BoundingSphereEditor = boundingSphereEditor;
            this.GeometryMeshEditor = geometryMeshEditor;
        }

        public override bool Draw(string name, Func<GeometryModel> get, Action<GeometryModel> set)
        {
            var model = get();

            var changed = this.BoundingSphereEditor.Draw($"{name}.Bounds", () => model.Bounds, s => { });

            for (var i = 0; i < model.Meshes.Count; i++)
            {
                var mesh = model.Meshes[i];
                if (ImGui.TreeNode($"{name}.Meshes[{i}] ({mesh.Geometry.Name})"))
                {
                    changed |= this.GeometryMeshEditor.Draw($"{name}.Meshes[{i}]", () => mesh, m => { });
                    ImGui.TreePop();
                }
            }

            return changed;
        }
    }
}
