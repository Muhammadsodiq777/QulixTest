using Microsoft.EntityFrameworkCore;
using QulixTest.Core.Domain;
using QulixTest.Core.IRepositories;
using System.Linq.Expressions;
using X.PagedList;

namespace QulixTest.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRpository<T> where T : class
    {
        public readonly DatabaseDbContext _context;
        public readonly DbSet<T> _db;

        public GenericRepository(DatabaseDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(long id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public async Task Delete<U>(U id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes)
        {
            IQueryable<T> query = _db;

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking()
                              .FirstOrDefaultAsync(expression);
        }
        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderby != null)
            {
                query = orderby(query);
            }

            return await query.AsNoTracking()
                              .ToListAsync();
        }

        public async Task<IPagedList<T>> GetAllPaged(RequestParams requestParams, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking()
                              .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
