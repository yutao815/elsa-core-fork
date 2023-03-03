using Elsa.Features.Abstractions;
using Elsa.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Common.Features;

/// <summary>
/// A feature that adds support for the memory cache.
/// </summary>
public class MemoryCacheFeature : FeatureBase
{
    /// <inheritdoc />
    public MemoryCacheFeature(IModule module) : base(module)
    {
    }

    /// <inheritdoc />
    public override void Configure() => Services.AddMemoryCache();
}