using System.Text.Json;
using System.Text.Json.Serialization;
using Elsa.Workflows.Core.Services;

namespace Elsa.Workflows.Core.Models;

public class ActivityDescriptor
{
    public string ActivityType { get; init; } = default!;
    public string Category { get; init; } = default!;
    public string? DisplayName { get; init; }
    public string? Description { get; init; }
    public ICollection<InputDescriptor> Inputs { get; init; } = new List<InputDescriptor>();
    public ICollection<OutputDescriptor> Outputs { get; init; } = new List<OutputDescriptor>();

    [JsonIgnore] public Func<ActivityConstructorContext, IActivity> Constructor { get; init; } = default!;
    public ActivityKind Kind { get; set; } = ActivityKind.Action;
    public ICollection<Port> Ports { get; init; } = new List<Port>();
}

public record ActivityConstructorContext(JsonElement Element, JsonSerializerOptions SerializerOptions);