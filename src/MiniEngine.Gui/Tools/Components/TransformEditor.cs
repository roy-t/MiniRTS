using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Physics;
using MiniEngine.Gui.Tools.Vectors;

namespace MiniEngine.Gui.Tools.Components
{
    [Service]
    public sealed class TransformEditor : ATool<Transform>
    {
        public override string Name => "Transform";

        public override bool HeaderValue(ref Transform value, ToolState tool)
        {
            ImGui.Text($"p: {VectorUtils.ToShortString(value.Position)}, s: {VectorUtils.ToShortString(value.Scale)}");
            return false;
        }

        public override bool Details(ref Transform value, ToolState tool)
        {
            var changed = false;

            var translation = value.Position;
            var scale = value.Scale;
            var origin = value.Origin;

            static bool translate(ref Vector3 t) => ImGui.DragFloat3(ToolUtils.NoLabel, ref t);
            changed |= ToolUtils.DetailsRow("Translation", ref translation, translate);

            static bool size(ref Vector3 s) => ImGui.DragFloat3("scale", ref s);
            changed |= ToolUtils.DetailsRow("Scale", ref scale, size);

            static bool pivot(ref Vector3 s) => ImGui.DragFloat3("origin", ref s);
            changed |= ToolUtils.DetailsRow("Origin", ref origin, pivot);

            static bool rot(ref float y) => ImGui.DragFloat(ToolUtils.NoLabel, ref y, MathHelper.TwoPi / 360.0f);

            var yaw = 0.0f;
            changed |= ToolUtils.DetailsRow("Yaw", ref yaw, rot);

            var pitch = 0.0f;
            changed |= ToolUtils.DetailsRow("Pitch", ref pitch, rot);

            var roll = 0.0f;
            changed |= ToolUtils.DetailsRow("Roll", ref roll, rot);

            if (ToolUtils.ButtonRow("Rotation", "Reset"))
            {
                value.SetRotation(Quaternion.Identity);
                return true;
            }

            if (changed)
            {
                value.MoveTo(translation);
                value.SetScale(scale);
                value.SetOrigin(origin);

                value.ApplyRotation(Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
                return true;
            }

            return false;
        }
    }
}
