using Elsa.Common.Entities;

namespace Elsa.Common.Services;

/// <summary>
/// A memory store.
/// </summary>
public class MemoryStore<TEntity>
{
    private IDictionary<string, TEntity> Entities { get; set; } = new Dictionary<string, TEntity>();
    
    /// <summary>
    /// Saves the specified entity.
    /// </summary>
    public void Save(TEntity entity, Func<TEntity, string> idAccessor) => Entities[idAccessor(entity)] = entity;

    /// <summary>
    /// Saves the specified entities.
    /// </summary>
    public void SaveMany(IEnumerable<TEntity> entities, Func<TEntity, string> idAccessor)
    {
        foreach (var entity in entities)
            Save(entity, idAccessor);
    }

    /// <summary>
    /// Gets the entity matching the specified predicate.
    /// </summary>
    public TEntity? Find(Func<TEntity, bool> predicate) => Entities.Values.Where(predicate).FirstOrDefault();
    
    /// <summary>
    /// Returns the entities matching the specified predicate.
    /// </summary>
    public IEnumerable<TEntity> FindMany(Func<TEntity, bool> predicate) => Entities.Values.Where(predicate);
    
    /// <summary>
    /// Returns the entities matching the specified predicate, ordered by the specified key.
    /// </summary>
    public IEnumerable<TEntity> FindMany<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBy, OrderDirection orderDirection = OrderDirection.Ascending)
    {
        var query = Entities.Values.Where(predicate);

        query = orderDirection switch
        {
            OrderDirection.Ascending => query.OrderBy(orderBy),
            OrderDirection.Descending => query.OrderByDescending(orderBy),
            _ => query.OrderBy(orderBy)
        };
        
        return query;
    }

    /// <summary>
    /// Returns all entities.
    /// </summary>
    public IEnumerable<TEntity> List() => Entities.Values;

    /// <summary>
    /// Deletes the entity matching the specified ID.
    /// </summary>
    public bool Delete(string id) => Entities.Remove(id);

    /// <summary>
    /// Deletes the entities matching the specified predicate.
    /// </summary>
    public int DeleteWhere(Func<TEntity, bool> predicate)
    {
        var query =
            from entry in Entities
            where predicate(entry.Value)
            select entry;

        var entries = query.ToList();
        foreach (var entry in entries)
            Entities.Remove(entry);

        return entries.Count;
    }

    /// <summary>
    /// Deletes the entities matching the specified IDs.
    /// </summary>
    public int DeleteMany(IEnumerable<string> ids)
    {
        var count = 0;
        foreach (var id in ids)
        {
            count++;
            Entities.Remove(id);
        }

        return count;
    }

    /// <summary>
    /// Deletes the specified entities.
    /// </summary>
    public int DeleteMany(IEnumerable<TEntity> entities, Func<TEntity, string> idAccessor)
    {
        var count = 0;
        var list = entities.ToList();

        foreach (var entity in list)
        {
            count++;
            var id = idAccessor(entity);
            Entities.Remove(id);
        }

        return count;
    }

    /// <summary>
    /// Returns all entities matching the specified query.
    /// </summary>
    public IEnumerable<TEntity> Query(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
    {
        var queryable = Entities.Values.AsQueryable();
        return query(queryable);
    }

    /// <summary>
    /// Returns true if there is at least one entity matching the specified predicate.
    /// </summary>
    public bool AnyAsync(Func<TEntity, bool> predicate) => Entities.Values.Any(predicate);
}