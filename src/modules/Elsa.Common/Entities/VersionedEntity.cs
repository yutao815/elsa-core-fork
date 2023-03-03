namespace Elsa.Common.Entities;

/// <summary>
/// Represents an entity with versioning information.
/// </summary>
public abstract class VersionedEntity : Entity
{
    /// <summary>
    /// Gets or sets the entity creation date.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the entity version number.
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the entity IsLatest flag.
    /// </summary>
    public bool IsLatest { get; set; }
    
    /// <summary>
    /// Gets or sets the entity IsPublished flag.
    /// </summary>
    public bool IsPublished { get; set; }
}