using Domain.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Store
{

    public interface IBaseStore<T>
    {
        Task<T> Add(T entity);
        Task<List<T>> Add(IEnumerable<T> entities);
        Task<List<T>> GetAll();
        Task<T> Get(string id);
        Task<List<T>> Get(IEnumerable<string> ids);
        Task<List<T>> GetBy(Func<T, bool> predicate);
        Task<T> GetFirst(Func<T, bool> predicate);
        Task<List<T>> GetByOrder<TKey>(Func<T, bool> selector, Func<T, TKey> order, bool isDesc = true);
        Task<List<T>> GetCustom<TKey>(Func<T, bool> selector, Func<T, TKey> order, bool isDesc = true, int take = 10, int skip = 0);
        Task<int> Update(T entity);
        Task<int> Update(IEnumerable<T> entities);
        Task<int> DeleteSoft(T entity);
        Task<int> DeleteSoft(IEnumerable<T> entities);
        Task<int> DeleteSoft(string id);
        Task<int> DeleteSoft(IEnumerable<string> ids);
        Task<int> Delete(string id);
        Task<int> Delete(IEnumerable<string> ids);
        Task<int> Delete(T entity);
        Task<int> Delete(IEnumerable<T> entities);

        Task<int> Count();
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }
    public abstract class BaseStore<T> : IBaseStore<T>
        where T : BaseEntity, new()
    {
        protected readonly ApiDbContext _context;
        protected DbSet<T> _collection;

        protected bool notDeleted(T entity) => entity.State != Domain.General.EntityState.Deleted;
        protected Func<T, bool> notDeleted() => x => notDeleted(x);

        public BaseStore(ApiDbContext context, Func<ApiDbContext, DbSet<T>> collection)
        {
            _context = context;
            _collection = collection(context);
        }

        public async Task<T> Add(T entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            entity.CreatedTime = DateTime.UtcNow;
            entity.State = Domain.General.EntityState.Active;
            entity.Id = Guid.NewGuid().ToString();

            await _collection.AddAsync(entity);
            await Save();
            return entity;
        }

        public async Task<List<T>> Add(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any()) return new List<T>();
            foreach (var e in entities)
            {
                e.Id = Guid.NewGuid().ToString();
                e.CreatedTime = DateTime.UtcNow;
                e.UpdatedTime = DateTime.UtcNow;
                e.State = Domain.General.EntityState.Active;
            }

            await _collection.AddRangeAsync(entities);
            await Save();

            return entities.ToList();
        }

        public async Task<List<T>> GetAll()
        {
            return await _collection?.ToAsyncEnumerable()?.ToList() ?? new List<T>();
        }

        public async Task<T> Get(string id)
        {
            return await _collection.FirstOrDefaultAsync(x => x.Id == id && notDeleted(x));
        }

        public async Task<T> GetFirst(Func<T, bool> predicate)
        {
            return await _collection.FirstOrDefaultAsync(x => predicate(x) && x.State == Domain.General.EntityState.Active);
        }

        public async Task<List<T>> Get(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any()) return null;
            return await _collection.Where(x => ids.Contains(x.Id) && notDeleted(x)).ToListAsync();
        }

        public async Task<List<T>> GetBy(Func<T, bool> predicate)
        {
            var result = await _collection.Where(predicate).Where(notDeleted()).ToAsyncEnumerable()?.ToList();
            return result ?? new List<T>();
        }

        public async Task<List<T>> GetByOrder<TKey>(Func<T, bool> selector, Func<T, TKey> order, bool isDesc = true)
        {
            var result = _collection.Where(selector).Where(notDeleted());

            if (isDesc)
                result = result.OrderByDescending(order);
            else
                result = result.OrderBy(order);

            return (await result.ToAsyncEnumerable()?.ToList()) ?? new List<T>();
        }

        public async Task<List<T>> GetCustom<TKey>(Func<T, bool> selector, Func<T, TKey> order, bool isDesc = true, int take = 10, int skip = 0)
        {
            take = Math.Max(1, take);
            skip = Math.Max(skip, 0);

            var result = _collection.Where(selector).Where(notDeleted);

            if (isDesc)
                result = result.OrderByDescending(order);
            else
                result = result.OrderBy(order);

            if (skip > 0)
                result = result.Skip(skip);

            return await result.Take(take).ToAsyncEnumerable()?.ToList() ?? new List<T>();

        }

        public async Task<int> Update(T entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return 0;
            }

            return 1;
        }

        public async Task<int> Update(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any()) return 0;

            int count = 0;

            try
            {
                _collection.UpdateRange(entities);
                count = await _context.SaveChangesAsync();
            }
            catch
            {
                count = 0;
            }

            return count;
        }

        public async Task<int> DeleteSoft(T entity)
        {
            entity.State = Domain.General.EntityState.Deleted;
            return await Update(entity);
        }

        public async Task<int> DeleteSoft(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any()) return 0;

            foreach (var e in entities)
            {
                e.State = Domain.General.EntityState.Deleted;
            }

            return await Update(entities);
        }

        public async Task<int> DeleteSoft(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return 0;

            var toDelete = await Get(id);
            if (toDelete == null) return 0;

            toDelete.State = Domain.General.EntityState.Deleted;
            return await Update(toDelete);
        }

        public async Task<int> DeleteSoft(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any()) return 0;

            var toDelete = await Get(ids);
            if (toDelete.Count == 0) return 0;

            foreach (var t in toDelete)
            {
                t.State = Domain.General.EntityState.Deleted;
            }

            return await Update(toDelete);
        }

        public async Task<int> Delete(string id)
        {
            try
            {
                var x = await Get(id);
                _collection.Remove(x);
                await Save();
            }
            catch
            {
                return 0;
            }

            return 1;
        }

        public async Task<int> Delete(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any()) return 0;
            var result = 0;
            try
            {
                var get = await Get(ids);
                _collection.RemoveRange(get);
                result = await _context.SaveChangesAsync();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        public async Task<int> Delete(T entity)
        {
            try
            {
                _collection.Remove(entity);
                return await Save();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> Delete(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any()) return 0;
            try
            {
                _collection.RemoveRange(entities);
                return await Save();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> Count()
        {
            return await _collection.CountAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await _collection.CountAsync(predicate);
        }

        protected async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
