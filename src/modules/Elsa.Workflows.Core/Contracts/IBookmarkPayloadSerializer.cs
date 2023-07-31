namespace Elsa.Workflows.Core.Contracts;

/// <summary>
/// Serializes and deserializes bookmark payloads.
/// </summary>
public interface IBookmarkPayloadSerializer
{
    /// <summary>
    /// Deserializes a JSON string to a bookmark payload of the specified type.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <typeparam name="T">The type of the bookmark payload.</typeparam>
    /// <returns>The deserialized bookmark payload.</returns>
    T Deserialize<T>(string json) where T : notnull;
    
    
    /// <summary>
    /// Deserializes a JSON string to a bookmark payload of the specified type.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <param name="type">The type of the bookmark payload.</param>
    /// <returns>The deserialized bookmark payload.</returns>
    object Deserialize(string json, Type type);
    
    
    /// <summary>
    /// Deserializes a JSON string to a bookmark payload using the type specified in the JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized bookmark payload.</returns>
    object Deserialize(string json);
    
    /// <summary>
    /// Serializes a bookmark payload to a JSON string.
    /// </summary>
    /// <param name="payload">The bookmark payload.</param>
    /// <typeparam name="T">The type of the bookmark payload.</typeparam>
    /// <returns>The serialized JSON string.</returns>
    string Serialize<T>(T payload) where T : notnull;
    
    /// <summary>
    /// Serializes a bookmark payload to a JSON string.
    /// </summary>
    /// <param name="payload">The bookmark payload.</param>
    /// <returns>The serialized JSON string.</returns>
    string Serialize(object payload);
}