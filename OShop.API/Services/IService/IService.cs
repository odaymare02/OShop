using System.Linq.Expressions;

namespace OShop.API.Services.IService
{
    public interface IService<T> where T :  class
    {
        //add here all function thath will repeate in all models 
        /*
       Task< IEnumerable<T>> GetAsync( Expression<Func<T,bool>>? expression=null, Expression<Func<T, object>>?[] includes = null,bool isTracked=true);
        Task<T>? GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[]includes=null, bool isTracked = true);
        Task<T> AddAsync(T item, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default);
        */

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>?[]includes=null,bool isTracked=true);
        Task<T>? GetOne(Expression<Func<T,bool>> expression, Expression<Func<T, object>>?[]includes=null,bool isTracked=true );
        Task<T> AddAsync(T entity, CancellationToken cancellation = default);
        Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
