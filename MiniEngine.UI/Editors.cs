using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives;
using MiniEngine.Units;
using System;
using Num = System.Numerics;

namespace MiniEngine.UI
{
    public static class Editors
    {        
        public static void CreateEditor(string label, object value, object min, object max, Action<object> setter)
        {                        
            if (setter == null)
            {
                ImGui.LabelText(label, value.ToString());
                return;
            }

            switch (value)
            {
                case Vector3 vector:
                    var v = ToNum(vector);
                    if (ImGui.SliderFloat3(label, ref v, (float)min, (float)max))
                    {
                        setter(ToXna(v));
                    }
                    break;


                case Pose pose:
                    var yaw = pose.Yaw;
                    var pitch = pose.Pitch;
                    var roll = pose.Roll;
                    var translation = ToNum(pose.Translation);
                    var scale = ToNum(pose.Scale);

                    var set = false;
                    set |= ImGui.DragFloat3("Translation", ref translation, 0.1f);
                    set |= ImGui.DragFloat3("Scale", ref scale, 0.01f);
                    set |= ImGui.SliderFloat("Yaw", ref yaw, -MathHelper.Pi, MathHelper.Pi);
                    set |= ImGui.SliderFloat("Pitch", ref pitch, -MathHelper.Pi, MathHelper.Pi);
                    set |= ImGui.SliderFloat("Roll", ref roll, -MathHelper.Pi, MathHelper.Pi);

                    if (set) { setter(new Pose(ToXna(translation), ToXna(scale), yaw, pitch, roll)); }
                    break;

                case Color color:
                    var c = ToNum(color.ToVector4());
                    if (ImGui.ColorEdit4(label, ref c))
                    {
                        setter(new Color(ToXna(c)));
                    }
                    break;

                case Seconds s:
                    var fs = s.Value;
                    if (ImGui.SliderFloat(label, ref fs, (float)min, (float)max))
                    {
                        setter(new Seconds(fs));
                    }
                    break;
                case float f:
                    if (ImGui.SliderFloat(label, ref f, (float)min, (float)max))
                    {
                        setter(f);
                    }
                    break;

                default:
                    ImGui.LabelText(label + "*", value.ToString());
                    return;
            }
        }
      
        private static Num.Vector3 ToNum(Vector3 v) => new Num.Vector3(v.X, v.Y, v.Z);
        private static Vector3 ToXna(Num.Vector3 v) => new Vector3(v.X, v.Y, v.Z);

        private static Num.Vector4 ToNum(Vector4 v) => new Num.Vector4(v.X, v.Y, v.Z, v.W);
        private static Vector4 ToXna(Num.Vector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);                
    }
}
