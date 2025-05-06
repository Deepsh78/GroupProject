using GroupApi.Data;
using Microsoft.EntityFrameworkCore;

namespace GroupApi.GenericClasses
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<TEntity> _entity;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _entity = _dbContext.Set<TEntity>();
        }

        #region Property

        public IQueryable<TEntity> Table => _entity.AsQueryable();

        public IQueryable<TEntity> TableNoTracking => _entity.AsNoTracking().AsQueryable();

        #endregion


        #region Methods

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _entity.AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _entity.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Update(entity);
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _entity.UpdateRange(entities);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _entity.RemoveRange(entities);
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _entity.FindAsync(id);
        }





        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        #endregion


    }
}
