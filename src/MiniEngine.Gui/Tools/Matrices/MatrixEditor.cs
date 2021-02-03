using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Matrices
{
    [Service]
    public sealed class MatrixEditor : ATool<Matrix>
    {
        public override string Name => "Matrix";

        public override bool HeaderValue(ref Matrix value, ToolState tool)
        {
            ImGui.Text(value.ToString());
            return false;
        }

        public override bool Details(ref Matrix value, ToolState tool)
        {
            value.Decompose(out var scale, out var quaternion, out var translation);

            // TODO: this doesn't seem to work always, unstable
            Decompose(quaternion, out var yaw, out var pitch, out var roll);

            var rotation = new Vector3(pitch, yaw, roll);

            var changed = false;

            static bool translate(ref Vector3 t) => ImGui.DragFloat3(ToolUtils.NoLabel, ref t);
            changed |= ToolUtils.DetailsRow("Translation", ref translation, translate);

            static bool rotate(ref Vector3 r) => ImGui.SliderFloat3(ToolUtils.NoLabel, ref r, -MathHelper.Pi, MathHelper.Pi);
            changed |= ToolUtils.DetailsRow("Rotation", ref rotation, rotate);

            static bool size(ref Vector3 s) => ImGui.DragFloat3("scale", ref s);
            changed |= ToolUtils.DetailsRow("Scale", ref scale, size);

            if (changed)
            {
                value = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.CreateTranslation(translation);
                return true;
            }

            return false;
        }

        private static void Decompose(Quaternion r, out float yaw, out float pitch, out float roll)
        {
            yaw = MathF.Atan2(2.0f * ((r.Y * r.W) + (r.X * r.Z)), 1.0f - (2.0f * ((r.X * r.X) + (r.Y * r.Y))));
            pitch = MathF.Asin(2.0f * ((r.X * r.W) - (r.Y * r.Z)));
            roll = MathF.Atan2(2.0f * ((r.X * r.Y) + (r.Z * r.W)), 1.0f - (2.0f * ((r.X * r.X) + (r.Z * r.Z))));
        }
    }
}
