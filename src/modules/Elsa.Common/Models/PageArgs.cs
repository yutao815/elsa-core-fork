namespace Elsa.Common.Models;

/// <summary>
/// Represents pagination arguments.
/// </summary>
/// <param name="Page">The page to request.</param>
/// <param name="PageSize">The page size to request.</param>
public record PageArgs(int? Page, int? PageSize)
{
    /// <summary>
    /// Gets the offset.
    /// </summary>
    public int? Offset => Page * PageSize;
    
    /// <summary>
    /// Gets the limit.
    /// </summary>
    public int? Limit => PageSize;

    /// <summary>
    /// Gets the arguments for the next page.
    /// </summary>
    public PageArgs Next() => this with { Page = Page + 1 };
}