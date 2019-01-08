using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VipcoSageX3.Services
{
    public interface IRepositorySageX3<TEntity> where TEntity : class
    {
        TEntity Get(string id, bool option = false);
        TEntity Get(int id, bool option = false);
        Task<TEntity> GetAsync(int id, bool option = false);
        Task<TEntity> GetAsync(string id, bool option = false);
        IQueryable<TEntity> GetAllAsQueryable();
        Task<ICollection<TEntity>> GetAllAsync(bool option = false);
        Task<ICollection<TEntity>> GetAllAsync<TProperty>(Expression<Func<TEntity, TProperty>> include, bool option = false);
        Task<ICollection<TEntity>> GetAllWithConditionAsync(Expression<Func<TEntity, bool>> match = null, bool option = false);
        TEntity Find(Expression<Func<TEntity, bool>> match, bool option = false);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, bool option = false);
        Task<TEntity> FindAsync<TProperty>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TProperty>> include, bool option = false);
        ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match, bool option = false);
        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, bool option = false);
        Task<ICollection<TEntity>> FindAllAsync<TProperty>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TProperty>> include, bool option = false);
        TEntity Add(TEntity nTEntity);
        Task<TEntity> AddAsync(TEntity nTEntity);
        Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> nTEntityList);
        Task<TEntity> UpdateAsync(TEntity updated, int key);
        Task<TEntity> UpdateAsync(TEntity updated, string key);
        TEntity Update(TEntity updated, string key);
        TEntity Update(TEntity updated, int key);
        void Delete(int key);
        void Delete(string key);
        Task<int> DeleteAsync(int key);
        Task<int> DeleteAsync(string key);
        Task<int> CountAsync();
        int CountWithMatch(Expression<Func<TEntity, bool>> match);
        Task<int> CountWithMatchAsync(Expression<Func<TEntity, bool>> match);
        Task<bool> AnyDataAsync(Expression<Func<TEntity, bool>> match);
        /////////
        // New //
        /////////
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"></param>
        /// <remarks>This method default no-tracking query.</remarks>
        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                   Expression<Func<TEntity, bool>> predicate = null,
                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                   Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                   bool disableTracking = true);
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"></param>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                     Expression<Func<TEntity, bool>> predicate = null,
                                     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                     bool disableTracking = true);
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="disableTracking"></param>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<ICollection<TResult>> GetToListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                    Expression<Func<TEntity, bool>> predicate = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                    int? skip = null, int? take = null,
                                    bool disableTracking = true);

        Task<int> GetLengthWithAsync(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true);
    }
}
