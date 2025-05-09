using System.Linq.Expressions;

namespace GroupApi.GenericClasses
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}