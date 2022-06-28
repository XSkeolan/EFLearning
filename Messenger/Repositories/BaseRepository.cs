using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Messenger.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly MessengerContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(MessengerContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> FindByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted == false);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            TEntity? entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted == false);
            if (entity == null)
            {
                throw new ArgumentException("Entity not found");
            }
            await DeleteAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetByConditions(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        protected async Task LoadCollection(TEntity entity, string propertyName)
        {
            await _context.Entry(entity).Collection(propertyName).LoadAsync();
        }
    }
}
