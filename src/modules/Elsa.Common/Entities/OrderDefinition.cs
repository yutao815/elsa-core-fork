using System.Linq.Expressions;

namespace Elsa.Common.Entities;

/// <summary>
/// Represents an order definition.
/// </summary>
public class OrderDefinition<T, TProp>
{
    /// <summary>
    /// Gets or sets the order direction.
    /// </summary>
    public OrderDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets the key selector.
    /// </summary>
    public Expression<Func<T, TProp>> KeySelector { get; set; } = default!;
}