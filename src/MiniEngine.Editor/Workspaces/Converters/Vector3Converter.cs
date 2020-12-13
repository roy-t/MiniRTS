using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MiniEngine.Editor.Workspaces.Converters
{
    public sealed class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            var values = value?.Split(',');
            var numbers = values?.Select(n => float.Parse(n)).ToArray() ?? new float[] { 0, 0, 0 };
            return new Vector3(numbers[0], numbers[1], numbers[2]);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 vector, JsonSerializerOptions options)
        {
            var numbers = new float[] { vector.X, vector.Y, vector.Z };
            var values = numbers.Select(n => n.ToString());
            var value = string.Join(',', values);
            writer.WriteStringValue(value);
        }
    }
}
