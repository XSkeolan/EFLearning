using MessengerDAL.Models;
using System.Linq.Expressions;

namespace Messenger.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Task CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteByIdAsync(Guid id);
        Task<TEntity?> FindByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetByConditions(Expression<Func<TEntity, bool>> predicate);

        //Task LoadCollection(TEntity entity, string propertyName);
        Task UpdateAsync(TEntity entity);
    }
}