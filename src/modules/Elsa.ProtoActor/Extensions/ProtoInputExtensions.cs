using System.Text.Json;
using System.Text.Json.Serialization;
using Elsa.Common.JsonConverters;
using Elsa.ProtoActor.Protos;

namespace Elsa.ProtoActor.Extensions;

internal static class ProtoInputExtensions
{
    public static IDictionary<string, object> Deserialize(this Input input) => input.Data.ToDictionary(x => x.Key, x => JsonSerializer.Deserialize<object>(x.Value.Text)!);

    public static Input Serialize(this IDictionary<string, object> input)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            
        };
        jsonOptions.Converters.Add(new TypeConverter());

        var result = new Input();
        var data = result.Data;

        foreach (var (key, value) in input)
        {
            data[key] = new Json
            {
                Text = JsonSerializer.Serialize(value, jsonOptions)
            };
        }

        return result;
    }
}