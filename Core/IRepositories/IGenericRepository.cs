using QulixTest.Core.Domain;
using System.Linq.Expressions;
using X.PagedList;

namespace QulixTest.Core.IRepositories
{
    public interface IGenericRpository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            List<string> includes = null
            );

        Task<IPagedList<T>> GetAllPaged(RequestParams requestParams, List<string> includes = null);

        Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes);

        Task Insert(T entity);

        Task InsertRange(IEnumerable<T> entities);

        Task Delete(long id);

        Task Delete<U>(U id);

        void DeleteRange(IEnumerable<T> entities);

        void Update(T entity);
    }
}
