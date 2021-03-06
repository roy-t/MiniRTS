﻿using System;
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
            ImGui.Text(VectorUtils.ToShortString(value));
            return false;
        }

        public override bool Details(ref Vector3 value, ToolState tool)
        {
            var pitch = (float)Math.Asin(-value.Y);
            var yaw = (float)Math.Atan2(value.X, value.Z);
            var changed = DetailsRow("Yaw", ref yaw);
            changed |= DetailsRow("Pitch", ref pitch);

            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
            value = -Vector3.TransformNormal(Vector3.Forward, rotation);
            return changed;
        }

        private static bool DetailsRow(string name, ref float value)
        {
            static bool action(ref float v) => ImGui.SliderFloat(ToolUtils.NoLabel, ref v, -MathHelper.Pi, MathHelper.Pi);
            return ToolUtils.DetailsRow(name, ref value, action);
        }
    }
}
