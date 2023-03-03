namespace Elsa.Common.Models;

/// <summary>
/// Represents a page of items.
/// </summary>
public record Page<T>(ICollection<T> Items, long TotalCount);

/// <summary>
/// Provides factory methods for <see cref="Page{T}"/>.
/// </summary>
public static class Page
{
    /// <summary>
    /// Creates a new instance of <see cref="Page{T}"/>.
    /// </summary>
    public static Page<T> Of<T>(ICollection<T> items, long totalCount) => new(items, totalCount);
}