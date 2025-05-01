
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OShop.API.Services.IService
{
    public class Service<T> : IService<T> where T : class
    {
        /*
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbset;//dbset is the modle who make this operation in this class
        public Service(ApplicationDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();//to inject the who i'm

        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
           await _context.AddAsync(entity, cancellationToken);
           await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[] includes = null, bool isTracked = true)
        {
            var all = await GetAsync(expression, includes, isTracked);
            return all.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> expression,Expression<Func<T, object>>?[] includes = null,bool isTracked=true)
        {
            IQueryable<T> entities = _dbset;
            if(expression is not null)
            {
                entities = entities.Where(expression);
            }
            if(includes is not null)
            {
                foreach(var item in includes)
                {
                    entities = entities.Include(item);
                }
            }
            if (!isTracked)
            {
                entities.AsNoTracking();
            }
            return await entities.ToListAsync();
        }

        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            T? entityInDb = _dbset.Find(id);
            if (entityInDb == null) return false;
            _dbset.Remove(entityInDb);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        */
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbset;
        public Service(ApplicationDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellation = default)
        {
            await _context.AddAsync(entity,cancellation);
            await _context.SaveChangesAsync(cancellation);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>?[] includes = null, bool isTracked = true)
        {
            IQueryable<T> entities = _dbset;
            if(expression is not null)
            {
                entities = entities.Where(expression);
            }
            if(includes is not null)
            {
                foreach(var item in includes)
                {
                    entities = entities.Include(item);
                }
            }
            if(!isTracked)
            {
                entities.AsNoTracking();
            }
            return await entities.ToListAsync();
        }

        public async Task<T>? GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[] includes = null, bool isTracked = true)
        {
            var all = await GetAsync(expression, includes, isTracked);
            return all.FirstOrDefault();
        }

        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = _dbset.Find(id);
            if (entity == null)
            {
                return false;
            }
            _dbset.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
