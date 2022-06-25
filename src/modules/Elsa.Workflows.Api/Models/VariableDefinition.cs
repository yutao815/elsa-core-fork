namespace Elsa.Workflows.Api.Models;

public record VariableDefinition(string Id, string Name, string Type, string? Value, string? StorageDriverId);