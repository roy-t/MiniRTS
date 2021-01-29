using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Vectors
{
    [Service]
    public sealed class SliderNormalTool : ATool<Vector3>
    {
        public override string Name => "Normal";

        public override bool HeaderValue(ref Vector3 value, ToolState tool)
        {
            ImGui.Text($"{{X: {value.X:F2} Y: {value.Y:F2} Z: {value.Z:F2}}}");
            return false;
        }

        public override bool Details(ref Vector3 value, ToolState tool)
        {
            var pitch = (float)Math.Asin(-value.Y);
            var yaw = (float)Math.Atan2(value.X, value.Z);
            var changed = this.DetailsRow("Yaw", ref yaw);
            changed |= this.DetailsRow("Pitch", ref pitch);

            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
            value = -Vector3.TransformNormal(Vector3.Forward, rotation);
            return changed;
        }

        private bool DetailsRow(string name, ref float value)
        {
            static bool action(ref float v) => ImGui.SliderFloat(NoLabel, ref v, -MathHelper.PiOver2, MathHelper.PiOver2);
            return this.DetailsRow(name, ref value, action);
        }
    }
}
