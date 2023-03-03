namespace Elsa.Common.Entities;

/// <summary>
/// Represents an entity.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public string Id { get; set; } = default!;
}