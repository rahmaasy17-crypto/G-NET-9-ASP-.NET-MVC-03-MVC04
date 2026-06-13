using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.classes
{
    //LINQ queries > no Code Duplication
    //Repository Pattern
    public class GenaricRepository<TEntity> : IGenaricReposatory<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbContext;
        private readonly DbSet<TEntity> _set;
       public GenaricRepository(GymDbContext gymDbContext)
        {
            dbContext = gymDbContext;
            _set=dbContext.Set<TEntity>();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            _set.Add(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            _set.Remove(entity);
            return await dbContext.SaveChangesAsync();
        }


        public async Task<TEntity?> GetByIDAsync(int id, CancellationToken c = default)
        {
            return await _set.FindAsync(id, c);
        }


        public async Task<int> UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking, CancellationToken c)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync();
        }

       public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken n)
        {
            return _set.AsNoTracking().AnyAsync(predicate, n);  
        }
    }
}
