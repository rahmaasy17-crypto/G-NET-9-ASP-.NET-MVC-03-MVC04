using GymManagement.DAL.Data.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.interfaces
{
    public interface IGenaricReposatory<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken c = default);
        Task<TEntity?> GetByIDAsync(int id, CancellationToken c = default);
        Task<int> AddAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);
         Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate ,CancellationToken c);
    }

}
