using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class MatrixEditor : AEditor<Matrix>
    {
        public override bool Draw(string name, Func<Matrix> get, Action<Matrix> set)
        {
            var matrix = get();

            matrix.Decompose(out var scale, out var quaternion, out var translation);
            Decompose(quaternion, out var yaw, out var pitch, out var roll);

            var changed = false;
            changed |= ImGui.DragFloat3($"{name}.Translation", ref translation);
            changed |= ImGui.DragFloat3($"{name}.Scale", ref scale);
            changed |= ImGui.SliderAngle($"{name}.Yaw", ref yaw);
            changed |= ImGui.SliderAngle($"{name}.Pitch", ref pitch);
            changed |= ImGui.SliderAngle($"{name}.Roll", ref roll);

            if (changed)
            {
                set(Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(translation));
                return true;
            }

            return false;
        }

        public static void Decompose(Quaternion r, out float yaw, out float pitch, out float roll)
        {
            yaw = MathF.Atan2(2.0f * ((r.Y * r.W) + (r.X * r.Z)), 1.0f - (2.0f * ((r.X * r.X) + (r.Y * r.Y))));
            pitch = MathF.Asin(2.0f * ((r.X * r.W) - (r.Y * r.Z)));
            roll = MathF.Atan2(2.0f * ((r.X * r.Y) + (r.Z * r.W)), 1.0f - (2.0f * ((r.X * r.X) + (r.Z * r.Z))));
        }
    }
}
