using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniEngine.Systems;

namespace MiniEngine.Editor.Windows.Converters
{
    public sealed class EntityConverter : JsonConverter<Entity>
    {
        public override Entity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new Entity(reader.GetInt32());

        public override void Write(Utf8JsonWriter writer, Entity entity, JsonSerializerOptions options)
            => writer.WriteNumberValue(entity.Id);
    }
}
