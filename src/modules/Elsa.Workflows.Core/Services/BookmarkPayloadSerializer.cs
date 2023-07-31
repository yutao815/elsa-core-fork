using System.Text.Json;
using Elsa.Expressions.Contracts;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Serialization.Converters;

namespace Elsa.Workflows.Core.Services;

/// <inheritdoc />
public class BookmarkPayloadSerializer : IBookmarkPayloadSerializer
{
    private readonly JsonSerializerOptions _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookmarkPayloadSerializer"/> class.
    /// </summary>
    public BookmarkPayloadSerializer(IWellKnownTypeRegistry wellKnownTypeRegistry)
    {
        _settings = new JsonSerializerOptions
        {
            // Enables serialization of ValueTuples, which use fields instead of properties.
            IncludeFields = true,
            PropertyNameCaseInsensitive = true
        };
        
        _settings.Converters.Add(new TypeJsonConverter(wellKnownTypeRegistry));
        _settings.Converters.Add(new PolymorphicObjectConverterFactory());
    }

    /// <inheritdoc />
    public T Deserialize<T>(string json) where T : notnull => JsonSerializer.Deserialize<T>(json, _settings)!;

    /// <inheritdoc />
    public object Deserialize(string json, Type type) => JsonSerializer.Deserialize(json, type, _settings)!;

    /// <inheritdoc />
    public object Deserialize(string json) =>  JsonSerializer.Deserialize(json, typeof(object), _settings)!;

    /// <inheritdoc />
    public string Serialize<T>(T payload) where T : notnull => JsonSerializer.Serialize(payload, typeof(T), _settings);

    /// <inheritdoc />
    public string Serialize(object payload) => JsonSerializer.Serialize(payload, payload.GetType(), _settings);
}