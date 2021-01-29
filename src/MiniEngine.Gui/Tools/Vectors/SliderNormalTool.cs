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

        public override Vector3 HeaderValue(Vector3 value, ToolState tool)
        {
            ImGui.Text($"{{X: {value.X:F2} Y: {value.Y:F2} Z: {value.Z:F2}}}");
            return value;
        }

        public override Vector3 Details(Vector3 value, ToolState tool)
        {
            var pitch = (float)Math.Asin(-value.Y);
            var yaw = (float)Math.Atan2(value.X, value.Z);
            yaw = this.DetailsRow("Yaw", yaw);
            pitch = this.DetailsRow("Pitch", pitch);

            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
            return -Vector3.TransformNormal(Vector3.Forward, rotation);
        }

        private float DetailsRow(string name, float value)
        {
            float action()
            {
                ImGui.SliderFloat(NoLabel, ref value, -MathHelper.PiOver2, MathHelper.PiOver2);
                return value;
            }
            return this.DetailsRow(name, action);
        }
    }
}
