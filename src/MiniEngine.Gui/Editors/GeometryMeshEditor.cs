using System;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class GeometryMeshEditor : AEditor<GeometryMesh>
    {
        private readonly MaterialEditor MaterialEditor;
        private readonly MatrixEditor MatrixEditor;

        public GeometryMeshEditor(MaterialEditor materialEditor, MatrixEditor matrixEditor)
        {
            this.MaterialEditor = materialEditor;
            this.MatrixEditor = matrixEditor;
        }

        public override bool Draw(string name, Func<GeometryMesh> get, Action<GeometryMesh> set)
        {
            var mesh = get();

            // Geometry

            var changed = false;

            changed |= this.MaterialEditor.Draw($"{name}.Material", () => mesh.Material, m => { });
            changed |= this.MatrixEditor.Draw($"{name}.Offset", () => mesh.Offset, m => { });

            return changed;
        }
    }
}
