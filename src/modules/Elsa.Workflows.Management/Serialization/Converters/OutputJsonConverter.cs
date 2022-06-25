using System.Text.Json;
using System.Text.Json.Serialization;
using Elsa.Expressions.Models;
using Elsa.Expressions.Services;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Core.Services;

namespace Elsa.Workflows.Management.Serialization.Converters;

/// <summary>
/// Serializes <see cref="Input"/> objects.
/// </summary>
public class OutputJsonConverter<T> : JsonConverter<Output<T>?>
{
    private readonly IWellKnownTypeRegistry _wellKnownTypeRegistry;

    public OutputJsonConverter(IWellKnownTypeRegistry wellKnownTypeRegistry)
    {
        _wellKnownTypeRegistry = wellKnownTypeRegistry;
    }

    public override bool CanConvert(Type typeToConvert) => typeof(Output).IsAssignableFrom(typeToConvert);

    public override Output<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
            return null;

        if (!doc.RootElement.TryGetProperty("type", out var outputTargetTypeElement))
            return null;

        if (!doc.RootElement.TryGetProperty("memoryReference", out var memoryReferenceElement))
            return default;

        if (!memoryReferenceElement.TryGetProperty("id", out var memoryReferenceIdElement))
            return default;

        var variable = new Variable(memoryReferenceIdElement.GetString()!);

        if (memoryReferenceElement.TryGetProperty("storageDriverId", out var storageDriverIdElement)) 
            variable.StorageDriverId = storageDriverIdElement.GetString();

        return (Output<T>)Activator.CreateInstance(typeof(Output<T>), variable)!;
    }

    public override void Write(Utf8JsonWriter writer, Output<T>? value, JsonSerializerOptions options)
    {
        var valueType = typeof(T);
        var valueTypeAlias = _wellKnownTypeRegistry.GetAliasOrDefault(valueType);
        var variable = value?.MemoryBlockReference as Variable;

        var model = new
        {
            Type = valueTypeAlias,
            MemoryReference = variable == null
                ? null
                : new
                {
                    Id = variable.Id,
                    StorageDriverId = variable.StorageDriverId
                }
        };

        JsonSerializer.Serialize(writer, model, options);
    }
}