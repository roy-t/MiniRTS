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

        public override Vector3 HeaderValue(Vector3 value)
        {
            ImGui.Text(value.ToString());
            return value;
        }

        public override Vector3 Details(Vector3 value, ToolState tool)
        {
            var pitch = (float)Math.Asin(-value.Y);
            var yaw = (float)Math.Atan2(value.X, value.Z);
            yaw = ColumnValue("Yaw", yaw);
            pitch = ColumnValue("Pitch", pitch);

            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
            return -Vector3.TransformNormal(Vector3.Forward, rotation);
        }

        //public override Vector3 Select(Vector3 value, Property property, ToolState tool)
        //{
        //    ImGui.Columns(2);
        //    ImGui.Separator();

        //    ImGui.AlignTextToFramePadding();
        //    bool open = ImGui.TreeNode(property.Name);
        //    ImGui.NextColumn();
        //    ImGui.AlignTextToFramePadding();
        //    ImGui.Text(value.ToString());

        //    ImGui.NextColumn();
        //    if (open)
        //    {

        //        var pitch = (float)Math.Asin(-value.Y);
        //        var yaw = (float)Math.Atan2(value.X, value.Z);
        //        yaw = ColumnValue("Yaw", yaw);
        //        pitch = ColumnValue("Pitch", pitch);

        //        var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
        //        value = -Vector3.TransformNormal(Vector3.Forward, rotation);

        //        ImGui.TreePop();
        //    }

        //    ImGui.Columns(1);
        //    ImGui.Separator();
        //    return value;
        //}

        private static float ColumnValue(string name, float value)
        {
            ImGui.PushID(name.GetHashCode());

            ImGui.AlignTextToFramePadding();
            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx(name, flags);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            ImGui.SliderFloat("##value", ref value, -MathHelper.PiOver2, MathHelper.PiOver2);

            ImGui.NextColumn();

            ImGui.PopID();
            return value;
        }
    }
}
