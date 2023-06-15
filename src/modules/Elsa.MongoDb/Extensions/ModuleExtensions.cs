using Elsa.Features.Services;
using Elsa.MongoDb.Features;
using Elsa.MongoDB.Options;
using JetBrains.Annotations;

namespace Elsa.MongoDb.Extensions;

/// <summary>
/// Extends <see cref="IModule"/> to configure the <see cref="MongoDbFeature"/> feature.
/// </summary>
[PublicAPI]
public static class ModuleExtensions
{
    /// <summary>
    /// Enables the <see cref="MongoDbFeature"/> feature.
    /// </summary>
    public static IModule UseMongoDb(
        this IModule module, 
        string connectionString,
        Action<MongoDbOptions> options,
        Action<MongoDbFeature>? configure = default)
    {
        configure += f => f.ConnectionString = connectionString;
        configure += f => f.Options += options;
        module.Configure(configure);
        return module;
    }
}