using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elsa.Common.JsonConverters;

public class TypeConverter : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) =>
        Type.GetType(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.AssemblyQualifiedName);
}