using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Units;
using System;
using Num = System.Numerics;

namespace MiniEngine.UI
{
    public static class Editors
    {
        public static void CreateEditor(string parent, string name, object value, object min, object max, Action<object> setter)
        {
            var label = $"{parent}::{name}";

            if (setter == null)
            {
                ImGui.LabelText(label, value.ToString());
                return;
            }

            switch (value)
            {
                case Vector3 vector:
                    var v = ToNumVector3(vector);                    
                    if (ImGui.SliderFloat3(label, ref v, (float)min, (float)max))
                    {
                        setter(ToXNAVector3(v));
                    }
                    break;


                case Quaternion quaternion:
                    Vector4 d;
                    quaternion.Deconstruct(out d.X, out d.Y, out d.Z, out d.W);
                    var vd = ToNumVector4(d);
                    if (ImGui.SliderFloat4(label, ref vd, -1.0f, 1.0f))
                    {
                        setter(new Quaternion(ToXNAVector4(vd)));
                    }

                    //var yawPitchRoll = QuaternionToYawPitchRoll(quaternion);
                    //if (ImGui.SliderAngle(label + " yaw", ref yawPitchRoll.X))
                    //{
                    //    setter(Quaternion.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z));
                    //}
                    //if (ImGui.SliderAngle(label + " pitch", ref yawPitchRoll.Y))
                    //{
                    //    setter(Quaternion.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z));
                    //}
                    //if (ImGui.SliderAngle(label + " roll", ref yawPitchRoll.Z))
                    //{
                    //    setter(Quaternion.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z));
                    //}
                    break;

                case Color color:
                    var c = ToNumVector4(color.ToVector4());
                    if (ImGui.ColorEdit4(label, ref c))
                    {
                        setter(new Color(ToXNAVector4(c)));
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

        private static Vector3 QuaternionToYawPitchRoll(Quaternion q)
        {            
            var yaw = (float)Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            var pitch = (float)Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y));
            var roll = (float)Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

            return new Vector3(yaw, pitch, roll);
        }

        private static Num.Vector3 ToNumVector3(Vector3 v) => new Num.Vector3(v.X, v.Y, v.Z);
        private static Vector3 ToXNAVector3(Num.Vector3 v) => new Vector3(v.X, v.Y, v.Z);

        private static Num.Vector4 ToNumVector4(Vector4 v) => new Num.Vector4(v.X, v.Y, v.Z, v.W);
        private static Vector4 ToXNAVector4(Num.Vector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);        
    }
}
