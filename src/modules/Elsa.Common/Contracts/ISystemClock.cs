namespace Elsa.Common.Contracts;

/// <summary>
/// Provides access to the current date and time.
/// </summary>
public interface ISystemClock
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}